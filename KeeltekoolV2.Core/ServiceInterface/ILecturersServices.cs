using KeeltekoolV2.Core.Domain;
using KeeltekoolV2.Core.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace KeeltekoolV2.Core.ServiceInterface
{
    public interface ILecturersServices
    {

        Task<Lecturer> Create(LectureDTO dto);
        Task<Lecturer> Update(LectureDTO dto);
        Task<Lecturer> DetailAsync(LectureDTO dto);
        Task<Lecturer> Delete(LectureDTO dto);

    }
}
