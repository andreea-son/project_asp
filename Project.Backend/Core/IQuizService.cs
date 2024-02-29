using Project.Backend.Common.Models;

namespace Project.Backend.Core;

public interface IQuizService
{
    Task<QuizResultDto> TakeQuizAsync(QuizTakeDto data, int userId);
    Task<QuizDto?> CreateQuizAsync(QuizDto data, int userId);
    Task<QuizDto?> UpdateQuizAsync(QuizDisplayDto data, int userId);
    Task<QuizDto?> PublishQuizAsync(int id);
    Task<IEnumerable<QuizDisplayDto>> GetAllAsync();
    Task<IEnumerable<QuizDisplayDto>> SearchAsync(string term);
    Task<QuizTakeDto> GetByIdAsync(int id);
    Task<IEnumerable<QuizResultDto>> GetUserResultsAsync(int userId);
    Task<IEnumerable<QuizResultDto>> GetScoreboardAsync();
    Task<IEnumerable<QuizDisplayDto>> DeleteByIdAsync(int id);
}
