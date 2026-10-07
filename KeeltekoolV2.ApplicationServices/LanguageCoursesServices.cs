using KeeltekoolV2.Core.Domain;
using KeeltekoolV2.Core.DTO;
using KeeltekoolV2.Core.ServiceInterface;
using KeeltekoolV2.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace KeeltekoolV2.ApplicationServices
{
    public class LanguageCoursesServices : ILanguageCoursesServices
    {
        private readonly KeeltekoolV2Context _context;

        public LanguageCoursesServices(KeeltekoolV2Context context)
        {
            _context = context;
        }

        public async Task<LanguageCourse?> Create(LanguageCourseDTO dto)
        {
            //kontrollitakse kas dto omab mingeid andmeid
            if (dto == null)
            {
                return null;
            }

            //kui nimetus või keel on tühi, siis kursust ei lisata
            if (string.IsNullOrWhiteSpace(dto.Nimetus) || string.IsNullOrWhiteSpace(dto.Keel))
            {
                return null;
            }

            LanguageCourse domain = new LanguageCourse();

            domain.Id = Guid.NewGuid();
            domain.Nimetus = dto.Nimetus;
            domain.Keel = dto.Keel;
            domain.Tase = dto.Tase ?? "";
            domain.Kirjeldus = dto.Kirjeldus ?? "";
            domain.CreatedAt = DateTime.Now;
            domain.ModifiedAt = DateTime.Now;

            await _context.LanguageCourses.AddAsync(domain);
            await _context.SaveChangesAsync();

            return domain;
        }

        public async Task<LanguageCourse> Update(LanguageCourseDTO dto)
        {
            return null;
        }

        public async Task<LanguageCourse> DetailsAsync(Guid id)
        {
            var result = await _context.LanguageCourses
                .FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }

        public async Task<LanguageCourse> Delete(Guid id)
        {
            return null;
        }
    }
}