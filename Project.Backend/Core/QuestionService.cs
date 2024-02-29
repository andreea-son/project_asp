using Dapper;
using Project.Backend.Common.Enums;
using Project.Backend.Common.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Project.Backend.Core;

public class QuestionService : IQuestionService
{
    private readonly IDbConnection dbConnection;

    public QuestionService(IDbConnection dbConnection)
    {
        this.dbConnection = dbConnection;
        this.dbConnection.ConnectionString = "Server=localhost;Database=project;Uid=project;Pwd=abc123;";
    }

    public async Task<QuizQuestionDto> AddQuestionAsync(QuizQuestionDto data, int userId)
    {
        if (string.IsNullOrWhiteSpace(data.QuestionName))
            throw new ArgumentException("Question Name cannot be empty!");
        else if (string.IsNullOrWhiteSpace(data.FirstOption))
            throw new ArgumentException("First Option cannot be empty!");
        else if (string.IsNullOrWhiteSpace(data.SecondOption))
            throw new ArgumentException("Second Option cannot be empty!");
        else if (string.IsNullOrWhiteSpace(data.CorrectAnswer))
            throw new ArgumentException("Correct Answer cannot be empty!");
        else if (data.CorrectAnswerPoints <= 0)
            throw new ArgumentException("Correct Answer Points must be a positive number!");
        else if (data.QuizId <= 0)
            throw new ArgumentException("No Quiz specified!");
        else
        {
            string?[] options = new[] { data.FirstOption, data.SecondOption, data.ThirdOption, data.FourthOption };
            if (options.Where(o => o == data.FirstOption).Count() > 1 || options.Where(o => o == data.SecondOption).Count() > 1
            || (options.Where(o => o == data.ThirdOption).Count() > 1 && data.ThirdOption != null) || (options.Where(o => o == data.FourthOption).Count() > 1 && data.FourthOption != null))
                throw new ArgumentException("Duplicate answers not allowed!");
            var result = await dbConnection.QueryAsync<QuizQuestionDbDto>($"SELECT qq.id FROM quizquestions AS qq LEFT JOIN quizzes AS q ON qq.quiz_id = q.id " +
                $"WHERE qq.question_name = @{nameof(data.QuestionName)} AND q.id = @{nameof(data.QuizId)};", new { data.QuestionName, data.QuizId });
            if (result.Any())
                throw new InvalidOperationException("A similar question has already been added for the specified quiz!");
            else
            {
                var quizUser = (await dbConnection.QueryAsync<QuizDbDto>($"SELECT user_id AS UserId FROM quizzes WHERE id = @{nameof(data.QuizId)};", new { data.QuizId })).First();
                string userRole = (await dbConnection.QueryAsync<string>($"SELECT role FROM users WHERE id = @{nameof(userId)};", new { userId })).First();
                if (quizUser.UserId != userId && userRole != "Admin")
                    throw new UnauthorizedAccessException("You can only add questions to your own quizzes!");
                int? newQuizQuestionId = await dbConnection.ExecuteScalarAsync<int>($"INSERT INTO quizquestions (quiz_id, question_name, first_option, second_option, third_option, fourth_option, correct_answer, score) " +
                    $"VALUES (@{nameof(data.QuizId)}, @{nameof(data.QuestionName)}, @{nameof(data.FirstOption)}, @{nameof(data.SecondOption)}, @{nameof(data.ThirdOption)}, @{nameof(data.FourthOption)}, @{nameof(data.CorrectAnswer)}, @{nameof(data.CorrectAnswerPoints)}); " +
                    $"SELECT last_insert_id();",
                    new { data.QuestionName, data.QuizId, data.FirstOption, data.SecondOption, data.ThirdOption, data.FourthOption, data.CorrectAnswer, data.CorrectAnswerPoints });
                var res = (await dbConnection.QueryAsync<QuizQuestionDbDto>($"SELECT id, quiz_id AS QuizId, question_name AS QuestionName, first_option AS FirstOption, second_option AS SecondOption, " +
                    $"third_option AS ThirdOption, fourth_option AS FourthOption, correct_answer AS CorrectAnswer, score AS CorrectAnswerPoints FROM quizquestions WHERE id = @{nameof(newQuizQuestionId)};", new { newQuizQuestionId })).First();


                return new QuizQuestionDto(res.Id, res.QuizId, res.QuestionName!, res.FirstOption!, res.SecondOption!, res.ThirdOption, res.FourthOption, res.CorrectAnswer!, res.CorrectAnswerPoints);
            }
        }
    }
}
