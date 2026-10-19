using KeeltekoolV2.Core.Domain;
using KeeltekoolV2.Core.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace KeeltekoolV2.Core.ServiceInterface
{
    public interface IStudentsServices
    {
        Task<Student?> Create(StudentDTO dto);
        Task<Student?> Update(StudentDTO dto);
        Task<Student?> DetailsAsync(Guid id);
        Task<Student?> Delete(Guid id);
    }
}
