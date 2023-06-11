using System;
using Npgsql;
using Dapper;


// DB:
// STUDENT: (KLUCZGLOWNY:{id}) {imie} {nazwisko} {data_urodzenia}
// YEAR: (KLUCZGLOWNY: {id}) {rok} {id_studenta}
// LECTURE: (KLUCZGLOWNY: {id}) {tag} {nazwa} {rok} {id_studenta}

namespace University
{
    public class DatabaseHandler
    {
        private static NpgsqlConnection connection;
        public DatabaseHandler()
        {
            var cs = "Host=localhost;Port=5432;Username=myuser;database=ProjectDB";
            try
            {
                connection = new NpgsqlConnection(cs);
                connection.Open();
                CreateTables();
            }
            catch (Exception e)
            {
                Console.Write("BLAD BAZY DANYCH");
            }
        }
        private static void CreateTables()
        {
            string checkIfCreatedquery = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'public';";
            bool checkifCreated = connection.ExecuteScalar<int?>(checkIfCreatedquery, new { }) > 0 ? true : false;
            if (checkifCreated)
            {
                return;
            }
            string CreateStudentQuery = "CREATE TABLE student(id SERIAL PRIMARY KEY, imie varchar(50), nazwisko varchar(50), data_urodzenia DATE)";
            string CreateYearQuery = "CREATE TABLE year(rok INT PRIMARY KEY, id_studenta INT REFERENCES student(id))";
            string CreateLectureQuery = "CREATE TABLE lecture(id SERIAL PRIMARY KEY, tag varchar(5), nazwa varchar(50), rok INT REFERENCES year(rok), id_studenta INT REFERENCES student(id))";
            connection.ExecuteScalar(CreateStudentQuery, new { });
            connection.ExecuteScalar(CreateYearQuery, new { });
            connection.ExecuteScalar(CreateLectureQuery, new { });
        }
        public static void AddStudent(string imie, string nazwisko, DateTime data_urodzenia)
        {
            string AddStudentQuery = "INSERT INTO student(imie, nazwisko, data_urodzenia) VALUES (@imie, @nazwisko, @data_urodzenia)";
            connection.ExecuteScalar(AddStudentQuery, new { imie = imie, nazwisko = nazwisko, data_urodzenia = data_urodzenia });
        }
        public static void AddYear(int rok, int id_studenta)
        {
            string AddYearQuery = "INSERT INTO year(rok, id_studenta) VALUES (@rok, @id_studenta)";
            connection.ExecuteScalar(AddYearQuery, new { rok = rok, id_studenta = id_studenta });
        }
        public static void AddLecture(string tag, string nazwa, int rok, int id_studenta)
        {
            string AddLectureQuery = "INSERT INTO lecture(tag, nazwa, rok, id_studenta) VALUES (@tag, @nazwa, @rok, @id_studenta)";
            connection.ExecuteScalar(AddLectureQuery, new { tag = tag, nazwa = nazwa, rok = rok, id_studenta = id_studenta });
        }
        public static void DeleteStudent(int id)
        {
            string DeleteStudentQuery = "DELETE FROM student WHERE id = @id";
            connection.ExecuteScalar(DeleteStudentQuery, new { id = id });
        }
        public static void DeleteYear(int id)
        {
            string DeleteYearQuery = "DELETE FROM year WHERE id = @id";
            connection.ExecuteScalar(DeleteYearQuery, new { id = id });
        }
        public static void DeleteLecture(int id)
        {
            string DeleteLectureQuery = "DELETE FROM lecture WHERE id = @id";
            connection.ExecuteScalar(DeleteLectureQuery, new { id = id });
        }
        public static void UpdateStudent(int id, string imie, string nazwisko, DateTime data_urodzenia)
        {
            string UpdateStudentQuery = "UPDATE student SET imie = @imie, nazwisko = @nazwisko, data_urodzenia = @data_urodzenia WHERE id = @id";
            connection.ExecuteScalar(UpdateStudentQuery, new { id = id, imie = imie, nazwisko = nazwisko, data_urodzenia = data_urodzenia });
        }
        public static void UpdateYear(int id, int rok, int id_studenta)
        {
            string UpdateYearQuery = "UPDATE year SET rok = @rok, id_studenta = @id_studenta WHERE id = @id";
            connection.ExecuteScalar(UpdateYearQuery, new { id = id, rok = rok, id_studenta = id_studenta });
        }
        public static void UpdateLecture(int id, string tag, string nazwa, int rok, int id_studenta)
        {
            string UpdateLectureQuery = "UPDATE lecture SET tag = @tag, nazwa = @nazwa, rok = @rok, id_studenta = @id_studenta WHERE id = @id";
            connection.ExecuteScalar(UpdateLectureQuery, new { id = id, tag = tag, nazwa = nazwa, rok = rok, id_studenta = id_studenta });
        }
        public static List<Student> GetStudents()
        {
            string GetStudentsQuery = "SELECT * FROM student";
            return connection.Query<Student>(GetStudentsQuery, new { }).ToList();
        }
        public static List<Year> GetYears()
        {
            string GetYearsQuery = "SELECT * FROM year";
            return connection.Query<Year>(GetYearsQuery, new { }).ToList();
        }
        public static List<Lecture> GetLectures()
        {
            string GetLecturesQuery = "SELECT * FROM lecture";
            return connection.Query<Lecture>(GetLecturesQuery, new { }).ToList();
        }
        public static List<Student> GetStudentsByYear(int rok)
        {
            string GetStudentsByYearQuery = "SELECT * FROM student WHERE id IN (SELECT id_studenta FROM year WHERE rok = @rok)";
            return connection.Query<Student>(GetStudentsByYearQuery, new { rok = rok }).ToList();
        }
        public static List<Student> GetStudentsByLecture(string nazwa)
        {
            string GetStudentsByLectureQuery = "SELECT * FROM student WHERE id IN (SELECT id_studenta FROM lecture WHERE nazwa = @nazwa)";
            return connection.Query<Student>(GetStudentsByLectureQuery, new { nazwa = nazwa }).ToList();
        }
        public static List<Year> GetYearsByStudent(int id_studenta)
        {
            string GetYearsByStudentQuery = "SELECT * FROM year WHERE id_studenta = @id_studenta";
            return connection.Query<Year>(GetYearsByStudentQuery, new { id_studenta = id_studenta }).ToList();
        }
        public static List<Year> GetYearsByLecture(string nazwa)
        {
            string GetYearsByLectureQuery = "SELECT * FROM year WHERE rok IN (SELECT rok FROM lecture WHERE nazwa = @nazwa)";
            return connection.Query<Year>(GetYearsByLectureQuery, new { nazwa = nazwa }).ToList();
        }
        public static List<Lecture> GetLecturesByStudent(int id_studenta)
        {
            string GetLecturesByStudentQuery = "SELECT * FROM lecture WHERE id_studenta = @id_studenta";
            return connection.Query<Lecture>(GetLecturesByStudentQuery, new { id_studenta = id_studenta }).ToList();
        }
        public static List<Lecture> GetLecturesByYear(int rok)
        {
            string GetLecturesByYearQuery = "SELECT * FROM lecture WHERE rok = @rok";
            return connection.Query<Lecture>(GetLecturesByYearQuery, new { rok = rok }).ToList();
        }
        public static List<Student> GetStudentsByYearAndLecture(int rok, string nazwa)
        {
            string GetStudentsByYearAndLectureQuery = "SELECT * FROM student WHERE id IN (SELECT id_studenta FROM lecture WHERE rok = @rok AND nazwa = @nazwa)";
            return connection.Query<Student>(GetStudentsByYearAndLectureQuery, new { rok = rok, nazwa = nazwa }).ToList();
        }
        public static void EditLectureByStudentList(List<Student> students, string oldName, string newName, string oldTag, string newTag, int oldRok, int newRok)
        {
            foreach (Student student in students)
            {
                string EditLectureByStudentListQuery = "UPDATE lecture SET nazwa = @newName, tag = @newTag, rok = @newRok WHERE id_studenta = @id_studenta AND nazwa = @oldName AND tag = @oldTag AND rok = @oldRok";
                connection.ExecuteScalar(EditLectureByStudentListQuery, new { id_studenta = student.id, oldName = oldName, newName = newName, oldTag = oldTag, newTag = newTag, oldRok = oldRok, newRok = newRok });
            }
        }
        public static void EditYearByStudentList(List<Student> students, int oldRok, int newRok)
        {
            foreach (Student student in students)
            {
                string EditYearByStudentListQuery = "UPDATE year SET rok = @newRok WHERE id_studenta = @id_studenta AND rok = @oldRok";
                connection.ExecuteScalar(EditYearByStudentListQuery, new { id_studenta = student.id, oldRok = oldRok, newRok = newRok });
            }
        }

    }



}

