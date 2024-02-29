using Project.Backend.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Backend.Core;

public interface IQuestionService
{
    Task<QuizQuestionDto> AddQuestionAsync(QuizQuestionDto data, int userId);
}
