using KeeltekoolV2.Core.Domain;
using KeeltekoolV2.Core.DTO;
using KeeltekoolV2.Core.ServiceInterface;
using KeeltekoolV2.Data;
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
            return null;
        }

        public async Task<Student?> DetailsAsync(Guid id)
        {
            return null;
        }

        public async Task<Student?> Update(StudentDTO dto)
        {
            return null;
        }

        public async Task<Student?> Delete(Guid id)
        {
            return null;
        }
    }
}
