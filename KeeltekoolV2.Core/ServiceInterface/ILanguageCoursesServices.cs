using KeeltekoolV2.Core.Domain;
using KeeltekoolV2.Core.DTO;

namespace KeeltekoolV2.Core.ServiceInterface
{
    public interface ILanguageCoursesServices 
    {
        Task<LanguageCourse> Create(LanguageCourseDTO dto);
        Task<LanguageCourse> Update(LanguageCourseDTO dto);
        Task<LanguageCourse> Update(Guid id);
        Task<LanguageCourse> Delete(Guid id);
    }
}