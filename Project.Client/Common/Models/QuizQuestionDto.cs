using System.ComponentModel.DataAnnotations;

namespace Project.Client.Common.Models;

public record QuizQuestionDto(int Id, int QuizId, string QuestionName, string FirstOption, string SecondOption, string? ThirdOption, string? FourthOption,
   string CorrectAnswer, int CorrectAnswerPoints);