using KeeltekoolV2.Core.Domain;
using KeeltekoolV2.Core.DTO;
using KeeltekoolV2.Core.ServiceInterface;
using KeeltekoolV2.Data;

namespace KeeltekoolV2.ApplicationServices
{
    public class LanguageCoursesServices : ILanguageCoursesServices
    {
        private readonly KeeltekoolV2Context _context;   

        public LanguageCoursesServices(KeeltekoolV2Context context)
        {
            _context = context;
        }

        public Task<LanguageCourse> Create(LanguageCourseDTO dto) => Task.FromResult<LanguageCourse>(null!);
        public Task<LanguageCourse> Update(LanguageCourseDTO dto) => Task.FromResult<LanguageCourse>(null!);
        public Task<LanguageCourse> Update(Guid id) => Task.FromResult<LanguageCourse>(null!);
        public Task<LanguageCourse> Delete(Guid id) => Task.FromResult<LanguageCourse>(null!);
    }
}