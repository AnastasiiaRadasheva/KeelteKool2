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

            Lecturer lecturer = new Lecturer();

            lecturer.Id = Guid.NewGuid();
            lecturer.FirstName = dto.FirstName;
            lecturer.LastName = dto.LastName;
            lecturer.Qualifications = dto.Qualifications;
            lecturer.UserID = "";
            lecturer.CreatedAt = DateTime.Now;
            lecturer.ModifiedAt = DateTime.Now;

            await _context.Set<Lecturer>().AddAsync(lecturer);
            await _context.SaveChangesAsync();

            return lecturer;
        }

        public async Task<Lecturer?> DetailAsync(LectureDTO dto)
        {
            return await _context.Set<Lecturer>()
                .FirstOrDefaultAsync(x => x.Id == dto.Id);
        }

        public async Task<Lecturer?> Update(LectureDTO dto)
        {
            //leiame andmebaasist olemasoleva õppejõu
            var lecturer = await _context.Set<Lecturer>()
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (lecturer == null)
            {
                return null;
            }

            //muudame väljad, Id ja CreatedAt jäävad samaks
            lecturer.FirstName = dto.FirstName;
            lecturer.LastName = dto.LastName;
            lecturer.Qualifications = dto.Qualifications;
            lecturer.ModifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return lecturer;
        }

        public async Task<Lecturer?> Delete(LectureDTO dto)
        {
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

        public Task<List<Lecturer>> GetAll()
        {
            //TDD punane faas: meetod on olemas, et testid kompileeruks, aga tegevus puudub
            throw new NotImplementedException();
        }
    }
}