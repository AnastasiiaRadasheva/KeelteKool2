using KeeltekoolV2.Core.Domain;
using KeeltekoolV2.Core.DTO;
using KeeltekoolV2.Core.ServiceInterface;
using KeeltekoolV2.Data;
using Microsoft.EntityFrameworkCore;

namespace KeeltekoolV2.ApplicationServices.Servises
{
    public class LecturersServices : ILecturersServices
    {
        private readonly KeeltekoolV2Context _context;

        public LecturersServices(KeeltekoolV2Context context)
        {
            _context = context;
        }

        public async Task<Lecturer?> Create(LectureDTO dto)
        {
            //kontrollitakse kas dto omab mingeid andmeid
            if (dto == null)
            {
                return null;
            }

            //kui eesnimi või perenimi on tühi, siis õppejõudu ei lisata
            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
            {
                return null;
            }

            Lecturer lecturer = new Lecturer();

            lecturer.Id = Guid.NewGuid();
            //andmetelt eemaldatakse algusest ja lõpust üleliigsed tühikud
            lecturer.FirstName = dto.FirstName.Trim();
            lecturer.LastName = dto.LastName.Trim();
            lecturer.Qualifications = (dto.Qualifications ?? "").Trim();
            //seos kasutajakontoga, võib olla ka tühi
            lecturer.UserID = dto.UserID;
            lecturer.CreatedAt = DateTime.Now;
            lecturer.ModifiedAt = DateTime.Now;

            await _context.Set<Lecturer>().AddAsync(lecturer);
            await _context.SaveChangesAsync();

            return lecturer;
        }

        public async Task<Lecturer?> DetailAsync(LectureDTO dto)
        {
            if (dto == null)
            {
                return null;
            }

            return await _context.Set<Lecturer>()
                .FirstOrDefaultAsync(x => x.Id == dto.Id);
        }

        public async Task<Lecturer?> Update(LectureDTO dto)
        {
            if (dto == null)
            {
                return null;
            }

            //olemasolevaid andmeid ei tohi tühjade nimedega üle kirjutada
            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
            {
                return null;
            }

            //leiame andmebaasist olemasoleva õppejõu
            var lecturer = await _context.Set<Lecturer>()
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (lecturer == null)
            {
                return null;
            }

            //muudame väljad, Id ja CreatedAt jäävad samaks
            lecturer.FirstName = dto.FirstName.Trim();
            lecturer.LastName = dto.LastName.Trim();
            lecturer.Qualifications = (dto.Qualifications ?? "").Trim();
            lecturer.ModifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return lecturer;
        }

        public async Task<Lecturer?> Delete(LectureDTO dto)
        {
            if (dto == null)
            {
                return null;
            }

            var lecturer = await _context.Set<Lecturer>()
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (lecturer == null)
            {
                return null;
            }

            _context.Set<Lecturer>().Remove(lecturer);
            await _context.SaveChangesAsync();

            return lecturer;
        }

        public async Task<List<Lecturer>> GetAll()
        {
            return await _context.Set<Lecturer>().ToListAsync();
        }
    }
}
