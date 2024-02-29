using Dapper;
using Microsoft.IdentityModel.Abstractions;
using Project.Backend.Common.Enums;
using Project.Backend.Common.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Project.Backend.Core;

public class QuizService : IQuizService
{
    private readonly IDbConnection dbConnection;

    public QuizService(IDbConnection dbConnection)
    {
        this.dbConnection = dbConnection;
        this.dbConnection.ConnectionString = "Server=localhost;Database=project;Uid=project;Pwd=abc123;";
    }

    public async Task<QuizDto?> CreateQuizAsync(QuizDto data, int userId)
    {
        if (string.IsNullOrWhiteSpace(data.Name))
            throw new ArgumentException("Name cannot be empty!");
        else
        {
            var result = await dbConnection.QueryAsync<QuizDbDto>($"SELECT id FROM quizzes WHERE name = @{nameof(data.Name)} AND category = @{nameof(data.Category)};", new { data.Name, data.Category });
            if (result.Any())
                throw new InvalidOperationException("A quiz with the same name and category already exists!");
            else
            {
                int? newQuizId = await dbConnection.ExecuteScalarAsync<int>($"INSERT INTO quizzes (user_id, name, category, description) VALUES (@{nameof(userId)}, @{nameof(data.Name)}, @{nameof(data.Category)}, @{nameof(data.Description)}); " +
                    $"SELECT last_insert_id();",
                    new { userId, data.Name, Category = data.Category.ToString(), data.Description });
                var res = (await dbConnection.QueryAsync<QuizDbDto>($"SELECT id, name, category, description, published FROM quizzes WHERE id = @{nameof(newQuizId)};", new { newQuizId })).First();


                return new QuizDto(res.Name, Enum.Parse<Category>(res.Category!), res.Description, res.Published);
            }
        }
    }

    public async Task<QuizDto?> UpdateQuizAsync(QuizDisplayDto data, int userId)
    {
        if (string.IsNullOrWhiteSpace(data.Name))
            throw new ArgumentException("Name cannot be empty!");
        else
        {
            var result = await dbConnection.QueryAsync<QuizDbDto>($"SELECT id FROM quizzes WHERE id = @{nameof(data.Id)};", new { data.Id });
            if (!result.Any())
                throw new InvalidOperationException("There is no quiz with the specified id!");
            else
            {
                await dbConnection.ExecuteAsync($"UPDATE quizzes SET name = @{nameof(data.Name)}, category = @{nameof(data.Category)}, description = @{nameof(data.Description)}, published = @{nameof(data.Published)} WHERE id = @{nameof(data.Id)};",
                    new { data.Name, Category = data.Category.ToString(), data.Description, data.Published, data.Id });
                var res = (await dbConnection.QueryAsync<QuizDbDto>($"SELECT id, name, category, description, published FROM quizzes WHERE id = @{nameof(data.Id)};", new { data.Id })).First();

                return new QuizDto(res.Name, Enum.Parse<Category>(res.Category!), res.Description, res.Published);
            }
        }
    }

    public async Task<QuizDto?> PublishQuizAsync(int id)
    {
        var result = await dbConnection.QueryAsync<QuizDbDto>($"SELECT id FROM quizzes WHERE id = @{nameof(id)};", new { id });
        if (!result.Any())
            throw new InvalidOperationException("There is no quiz with the specified id!");
        else
        {
            await dbConnection.ExecuteAsync($"UPDATE quizzes SET published = true WHERE id = @{nameof(id)};",
                new { id });
            var res = (await dbConnection.QueryAsync<QuizDbDto>($"SELECT id, name, category, description, published FROM quizzes WHERE id = @{nameof(id)};", new { id })).First();

            return new QuizDto(res.Name, Enum.Parse<Category>(res.Category!), res.Description, res.Published);
        }
    }

    public async Task<QuizResultDto> TakeQuizAsync(QuizTakeDto data, int userId)
    {
        var returnResult = new QuizResultDto();
        returnResult.UserId = userId;
        returnResult.QuizId = data.Id;
        var result = (await dbConnection.QueryAsync<QuizDisplayDbDto>("SELECT id, user_id AS UserId, name, category, description " +
                $"FROM quizzes WHERE id = @{nameof(data.Id)};", new { data.Id }));
        if (result?.Any() == true)
        {
            var quizQuestions = await dbConnection.QueryAsync<QuizQuestionDbDto>($"SELECT id, quiz_id AS QuizId, question_name AS QuestionName, " +
            $"first_option AS FirstOption, second_option AS SecondOption, third_option AS ThirdOption, fourth_option AS FourthOption, " +
            $"correct_answer AS CorrectAnswer, score AS CorrectAnswerPoints FROM quizquestions WHERE quiz_id = @{nameof(data.Id)};", new { data.Id });
            if (quizQuestions?.Any() == true)
            {
                int score = 0;
                int correctQuestions = 0;
                if (data.Answers?.Any() == true)
                {
                    foreach (var question in quizQuestions)
                    {
                        foreach (var answer in data.Answers)
                        {
                            if (question.Id == answer.QuestionId)
                            {
                                if (question.CorrectAnswer?.ToLower() == answer.Answer?.ToLower())
                                {
                                    score += question.CorrectAnswerPoints;
                                    correctQuestions++;
                                }
                            }
                        }
                    }
                }
                returnResult.NumberOfQuestions = quizQuestions?.Count() ?? 0;
                returnResult.NumberOfCorrectQuestions = correctQuestions;
                returnResult.Score = score;
                returnResult.Id = await dbConnection.ExecuteScalarAsync<int>($"INSERT INTO userquizzes (user_id, quiz_id, nr_of_questions, nr_of_correct_questions, score) VALUES (@{nameof(userId)}, @{nameof(data.Id)}, @nrQuestions, @nrCorrectQuestions, @{nameof(score)}); " +
                  $"SELECT last_insert_id();",
                  new { userId, data.Id, nrQuestions = quizQuestions?.Count() ?? 0, nrCorrectQuestions = correctQuestions, score });
            }
            else
                throw new InvalidOperationException("Specified quiz has no questions!");
        }
        else
            throw new InvalidOperationException("Specified quiz does not exist!");
        returnResult.DateTaken = DateTime.Now;
        return returnResult;
    }

    public async Task<IEnumerable<QuizDisplayDto>> SearchAsync(string term)
    {
        var result = await dbConnection.QueryAsync<QuizDisplayDbDto>("SELECT q.id, q.user_id AS UserId, q.name, q.category, q.description, q.published, COUNT(qq.id) AS NrOfQuestions " +
            "FROM quizzes AS q " +
            "LEFT JOIN quizquestions AS qq ON q.id = qq.quiz_id " +
            $"WHERE q.name LIKE CONCAT('%', @{nameof(term)}, '%') GROUP BY q.id;", new { term });
        return result?.Select(dbQuiz => new QuizDisplayDto(dbQuiz.Id, dbQuiz.Published, dbQuiz.UserId, dbQuiz.Name, Enum.Parse<Category>(dbQuiz.Category!),
                dbQuiz.Description, dbQuiz.NrOfQuestions)) ?? Enumerable.Empty<QuizDisplayDto>();
    }

    public async Task<IEnumerable<QuizDisplayDto>> GetAllAsync()
    {
        var result = (await dbConnection.QueryAsync<QuizDisplayDbDto>("SELECT q.id, q.user_id AS UserId, q.name, q.category, q.description, q.published, COUNT(qq.id) AS NrOfQuestions " +
            "FROM quizzes AS q " +
            "LEFT JOIN quizquestions AS qq ON q.id = qq.quiz_id GROUP BY q.id;"));
        return result?.Select(dbQuiz => new QuizDisplayDto(dbQuiz.Id, dbQuiz.Published, dbQuiz.UserId, dbQuiz.Name, Enum.Parse<Category>(dbQuiz.Category!),
                dbQuiz.Description, dbQuiz.NrOfQuestions)) ?? Enumerable.Empty<QuizDisplayDto>();
    }

    public async Task<QuizTakeDto> GetByIdAsync(int id)
    {
        var returnResult = new QuizTakeDto();
        var result = (await dbConnection.QueryAsync<QuizDisplayDbDto>("SELECT id, user_id AS UserId, name, category, description " +
           $"FROM quizzes WHERE id = @{nameof(id)};", new { id }));
        if (result?.Any() == true)
        {
            var dbQuiz = result.First();
            returnResult.Id = dbQuiz.Id;
            returnResult.UserId = dbQuiz.UserId;
            returnResult.Category = Enum.Parse<Category>(dbQuiz.Category!);
            returnResult.Description = dbQuiz.Description;
            returnResult.Name = dbQuiz.Name;
        }
        var resultQuestions = await dbConnection.QueryAsync<QuizQuestionDbDto>($"SELECT id, quiz_id AS QuizId, question_name AS QuestionName, " +
            $"first_option AS FirstOption, second_option AS SecondOption, third_option AS ThirdOption, fourth_option AS FourthOption, " +
            $"correct_answer AS CorrectAnswer, score AS CorrectAnswerPoints FROM quizquestions WHERE quiz_id = @{nameof(id)};", new { id });

        returnResult.QuizQuestions = resultQuestions.Select(q =>
            new QuizQuestionDto(q.Id, q.QuizId, q.QuestionName!, q.FirstOption!, q.SecondOption!, q.ThirdOption, q.FourthOption, q.CorrectAnswer!, q.CorrectAnswerPoints)).ToArray();
        return returnResult;
    }

    public async Task<IEnumerable<QuizResultDto>> GetUserResultsAsync(int userId)
    {
        return await dbConnection.QueryAsync<QuizResultDto>("SELECT uq.id, uq.user_id AS UserId, u.username, q.name AS QuizName, uq.quiz_id AS QuizId, uq.score, uq.nr_of_questions AS NumberOfQuestions, uq.nr_of_correct_questions AS NumberOfCorrectQuestions, uq.date_taken AS DateTaken " +
        $"FROM userquizzes AS uq LEFT JOIN users AS u ON u.id = uq.user_id LEFT JOIN quizzes AS q ON q.id = uq.quiz_id WHERE uq.user_id = @{nameof(userId)};", new { userId });
    }

    public async Task<IEnumerable<QuizResultDto>> GetScoreboardAsync()
    {
        return await dbConnection.QueryAsync<QuizResultDto>("SELECT uq.id, uq.user_id AS UserId, u.username, q.name AS QuizName, uq.quiz_id AS QuizId, uq.score, uq.nr_of_questions AS NumberOfQuestions, uq.nr_of_correct_questions AS NumberOfCorrectQuestions, uq.date_taken AS DateTaken " +
        $"FROM userquizzes AS uq LEFT JOIN users AS u ON u.id = uq.user_id LEFT JOIN quizzes AS q ON q.id = uq.quiz_id WHERE (uq.nr_of_correct_questions >= CEIL(uq.nr_of_questions / 2)) ORDER BY uq.score DESC;");
    }

    public async Task<IEnumerable<QuizDisplayDto>> DeleteByIdAsync(int id)
    {
        if (dbConnection.State == ConnectionState.Closed)
            dbConnection.Open();
        var transaction = dbConnection.BeginTransaction();
        try
        {
            var deleteResult = (await dbConnection.ExecuteAsync($"DELETE FROM quizzes WHERE id = @{nameof(id)};", new { id }, transaction: transaction));
            deleteResult = (await dbConnection.ExecuteAsync($"DELETE FROM quizquestions WHERE quiz_id = @{nameof(id)};", new { id }, transaction: transaction));
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
        var result = (await dbConnection.QueryAsync<QuizDisplayDbDto>("SELECT q.id, q.user_id AS UserId, q.name, q.category, q.description, q.published, COUNT(qq.id) AS NrOfQuestions " +
           "FROM quizzes AS q " +
           "LEFT JOIN quizquestions AS qq ON q.id = qq.quiz_id GROUP BY q.id;"));
        return result?.Select(dbQuiz => new QuizDisplayDto(dbQuiz.Id, dbQuiz.Published, dbQuiz.UserId, dbQuiz.Name, Enum.Parse<Category>(dbQuiz.Category!),
                dbQuiz.Description, dbQuiz.NrOfQuestions)) ?? Enumerable.Empty<QuizDisplayDto>();
    }
}
