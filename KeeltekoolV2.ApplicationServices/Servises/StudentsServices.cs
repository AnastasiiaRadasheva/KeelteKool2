using KeeltekoolV2.Core.Domain;
using KeeltekoolV2.Core.DTO;
using KeeltekoolV2.Core.ServiceInterface;
using KeeltekoolV2.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace KeeltekoolV2.ApplicationServices.Servises
{
    public class StudentsServices : IStudentsServices
    {
        private readonly KeeltekoolV2Context _context;

        public StudentsServices(KeeltekoolV2Context context)
        {
            _context = context;
        }

        public async Task<Student?> Create(StudentDTO dto)
        {
            //kontrollitakse kas dto omab mingeid andmeid
            if (dto == null)
            {
                return null;
            }

            Student domain = new Student();

            domain.Id = Guid.NewGuid();
            domain.FirstName = dto.FirstName;
            domain.LastName = dto.LastName;
            domain.Email = dto.Email;
            domain.LanguageLevel = dto.LanguageLevel ?? "";
            domain.UserID = dto.UserID;
            domain.CreatedAt = DateTime.Now;
            domain.ModifiedAt = DateTime.Now;

            await _context.Students.AddAsync(domain);
            await _context.SaveChangesAsync();

            return domain;
        }

        public async Task<Student?> DetailsAsync(Guid id)
        {
            //otsime andmebaasist õpilase id järgi, kui ei leita tagastatakse null
            return await _context.Students
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Student?> Update(StudentDTO dto)
        {
            if (dto == null)
            {
                return null;
            }

            //leiame andmebaasist olemasoleva õpilase
            var domain = await _context.Students
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (domain == null)
            {
                return null;
            }

            //muudame väljad, CreatedAt jääb samaks
            domain.FirstName = dto.FirstName;
            domain.LastName = dto.LastName;
            domain.Email = dto.Email;
            domain.LanguageLevel = dto.LanguageLevel ?? "";
            domain.ModifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return domain;
        }

        public async Task<Student?> Delete(Guid id)
        {
            return null;
        }
    }
}
