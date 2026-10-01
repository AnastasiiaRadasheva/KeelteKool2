using KeeltekoolV2.Core.Domain;
using KeeltekoolV2.Core.DTO;
using KeeltekoolV2.Core.ServiceInterface;
using KeeltekoolV2.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace KeeltekoolV2.ApplicationServices
{
    public class LanguageCoursesServices : ILanguageCoursesServices
    {
        private readonly KeeltekoolV2Context _context;

        public LanguageCoursesServices(KeeltekoolV2Context context)
        {
            _context = context;
        }

        public async Task<LanguageCourse> Create(LanguageCourseDTO dto)
        {
            //kontrollitakse kas dto omab mingeid andmeid
            if (dto==null)
            {
                return null;
            }
            //tekitab uue andmebaasis istuva objekti vastava mudeli järgi
            LanguageCourse domain = new LanguageCourse();

            //omistab andmed andmeedastuseobjektist domeenimudelile

            //lahendades ära küsimused mis ei lahendatud kontrolleris, nagu näiteks id, createdat, modifiedat


            domain.Id = Guid.NewGuid();
            domain.Nimetus = dto.Nimetus;
            domain.Keel = dto.Keel;
            domain.Tase = dto.Tase ?? "";
            domain.Kirjeldus = dto.Kirjeldus ?? "";
            domain.CreatedAt = DateTime.Now;
            domain.ModifiedAt = DateTime.Now;
            //later, reguire user id to be attached to "ModifiedBy"parameterm to know who modified last

            //ToDo; check if db addition succeeded if yes return objectr if not null
            //teostatakse andmebaasi lisamise tegevus
       
            await _context.LanguageCourses.AddAsync(domain);
            await _context.SaveChangesAsync();
            //tagastatakse domeenile kuuluv objekt
            return domain;

        }

        public async Task<LanguageCourse> Update(LanguageCourseDTO dto)
        {
            return null;
        }

        public async Task<LanguageCourse> Update(Guid id)
        {
            return null;
        }

        public async Task<LanguageCourse> Delete(Guid id)
        {
            return null;
        }
    }
}