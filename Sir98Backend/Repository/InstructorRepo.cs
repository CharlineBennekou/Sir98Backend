using Sir98Backend.Models;
using System.Xml.Linq;

namespace Sir98Backend.Repository
{
    public class InstructorRepo
    {
        private readonly List<Instructor> _instructors = new();
        private int _nextId = 1;


        public InstructorRepo()
        {
            _instructors = new List<Instructor>();
            _instructors.Add(new Instructor { Id = _nextId++, Email = "larsboh@roskilde.dk", Number = "24629361", FirstName = "Lars", Image = "hansBillede.png" });
            _instructors.Add(new Instructor { Id = _nextId++, Email = "lillianv@roskilde.dk", Number = "30841920", FirstName = "Lillian", Image = "LiselotteBillede.png" });
            _instructors.Add(new Instructor { Id = _nextId++, Email = "pietherlh@roskilde.dk", Number = "61246799", FirstName = "Piether", Image = "JeppeBillede.png" });

        }

        public List<Instructor> GetAll()
        {
            return _instructors;
        }

        public Instructor? GetById(int id)
        {
            return _instructors.FirstOrDefault(i => i.Id == id);
        }

        public Instructor Add(Instructor instructor)
        {
            instructor.Id = _nextId++;
            _instructors.Add(instructor);
            return instructor;
        }

        public Instructor? Delete(int id)
        {
            var instructor = GetById(id);
            if (instructor != null)
            {
                _instructors.Remove(instructor);
            }
            return instructor;
        }

        public Instructor? Update(int id, Instructor instructor)
        {
            var existing = GetById(id);
            if (existing == null)
                return null;

            existing.Email = instructor.Email;
            existing.Number = instructor.Number;
            existing.FirstName = instructor.FirstName;
            existing.Image = instructor.Image;
            return existing;
        }
    }
}
