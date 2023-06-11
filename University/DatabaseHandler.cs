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
            string CreateYearQuery = "CREATE TABLE year(id SERIAL PRIMARY KEY, rok INT, id_studenta INT REFERENCES student(id))";
            string CreateLectureQuery = "CREATE TABLE lecture(id SERIAL PRIMARY KEY, tag varchar(5), nazwa varchar(50), id_roku INT REFERENCES year(id), id_studenta INT REFERENCES student(id))";
            connection.ExecuteScalar(CreateStudentQuery, new { });
            connection.ExecuteScalar(CreateYearQuery, new { });
            connection.ExecuteScalar(CreateLectureQuery, new { });
        }
        public static void AddStudent(string imie, string nazwisko, DateTime data_urodzenia)
        {
            string AddStudentQuery = "INSERT INTO student(imie, nazwisko, data_urodzenia) VALUES (@imie, @nazwisko, @data_urodzenia)";
            connection.ExecuteScalar(AddStudentQuery, new { imie = imie, nazwisko = nazwisko, data_urodzenia = data_urodzenia });
        }
        public static void AddYear(int rok, int id_student){
            string AddYearQuery = "INSERT INTO year(rok, id_studenta) VALUES (@rok, @id_student)";
            connection.ExecuteScalar(AddYearQuery, new { rok = rok, id_student = id_student });
        }
        public static void AddLecture(string tag, string nazwa, int id_roku, int id_studenta)
        {
            string AddLectureQuery = "INSERT INTO lecture(tag, nazwa, id_roku, id_studenta) VALUES (@tag, @nazwa, @id_roku, @id_studenta)";
            connection.ExecuteScalar(AddLectureQuery, new { tag = tag, nazwa = nazwa, id_roku = id_roku, id_studenta = id_studenta });
        }
        public static void DeleteStudent(int id)
        {
            string DeleteStudentQuery = "DELETE FROM student WHERE id = @id";
            connection.ExecuteScalar(DeleteStudentQuery, new { id = id });
        }
        public static void DeleteYear(int id){
            string DeleteYearQuery = "DELETE FROM year WHERE id = @id";
            connection.ExecuteScalar(DeleteYearQuery, new { id = id });
        }
        public static void DeleteLecture(int id)
        {
            string DeleteLectureQuery = "DELETE FROM lecture WHERE id = @id";
            connection.ExecuteScalar(DeleteLectureQuery, new { id = id });
        }
        public static void DeleteLectureForStudent(int id_lecture, int id_student){
            string DeleteLectureForStudentQuery = "DELETE FROM lecture WHERE id = @id_lecture AND id_studenta = @id_student";
            connection.ExecuteScalar(DeleteLectureForStudentQuery, new { id_lecture = id_lecture, id_student = id_student });
        }
        public static void DeleteYearForStudent(int id_roku, int id_studenta){
            string DeleteYearForStudentQuery = "DELETE FROM year WHERE id = @id_roku AND id_studenta = @id_studenta";
            connection.ExecuteScalar(DeleteYearForStudentQuery, new { id_roku = id_roku, id_studenta = id_studenta });
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
            var ans = connection.Query<Lecture>(GetLecturesQuery, new { }).ToList();
            foreach(var lect in ans)
            {
                lect.getRok();
            }
            return ans;
        }
        public static List<Student> GetStudentsByYear(int id_roku)
        {
            string GetStudentsByYearQuery = "SELECT * FROM student WHERE id IN (SELECT id_studenta FROM year WHERE id = @id_roku)";
            return connection.Query<Student>(GetStudentsByYearQuery, new { id_roku = id_roku }).ToList();
        }
        public static List<Student> GetStudentsByLecture(string nazwa)
        {
            string GetStudentsByLectureQuery = "SELECT * FROM student WHERE id IN (SELECT id_studenta FROM lecture WHERE nazwa = @nazwa)";
            return connection.Query<Student>(GetStudentsByLectureQuery, new { nazwa = nazwa }).ToList();
        }
        public static List<Year> GetMaximumStudentYear(int id_student){
            string GetMaximumStudentYearQuery = "SELECT * FROM year WHERE id_studenta = @id_student AND rok = (SELECT MAX(rok) FROM year WHERE id_studenta = @id_student)";
            return connection.Query<Year>(GetMaximumStudentYearQuery, new { id_student = id_student }).ToList();
        }
        public static List<Year> GetYearsByStudent(int id_studenta)
        {
            string GetYearsByStudentQuery = "SELECT * FROM year WHERE id_studenta = @id_studenta";
            return connection.Query<Year>(GetYearsByStudentQuery, new { id_studenta = id_studenta }).ToList();
        }
        public static List<Year> GetYearsByLecture(string nazwa)
        {
            string GetYearsByLectureQuery = "SELECT * FROM year WHERE id IN (SELECT id_roku FROM lecture WHERE nazwa = @nazwa)";
            return connection.Query<Year>(GetYearsByLectureQuery, new { nazwa = nazwa }).ToList();
        }
        public static List<Lecture> GetLecturesByStudent(int id_studenta)
        {
            string GetLecturesByStudentQuery = "SELECT * FROM lecture WHERE id_studenta = @id_studenta";
            var ans = connection.Query<Lecture>(GetLecturesByStudentQuery, new { id_studenta = id_studenta }).ToList();
            foreach(var a in ans)
            {
                a.getRok();
            }
            return ans;
        }
        public static List<Lecture> GetLecturesByYear(int id_roku)
        {
            string GetLecturesByYearQuery = "SELECT * FROM lecture WHERE id_roku = @id_roku";
            var ans = connection.Query<Lecture>(GetLecturesByYearQuery, new { id_roku = id_roku }).ToList();
            foreach (var a in ans)
            {
                a.getRok();
            }
            return ans;
        }
        public static List<Lecture> GetLecturesByRok(int rok){
            string GetLecturesByRokQuery = "SELECT * FROM lecture WHERE id_roku IN (SELECT id FROM year WHERE rok = @rok)";
            var ans = connection.Query<Lecture>(GetLecturesByRokQuery, new { rok = rok }).ToList();
            foreach (var a in ans)
            {
                a.getRok();
            }
            return ans;
        }
        public static List<Student> GetStudentsByYearAndLecture(int id_roku, string nazwa)
        {
            string GetStudentsByYearAndLectureQuery = "SELECT * FROM student WHERE id IN (SELECT id_studenta FROM lecture WHERE id_roku = @id_roku AND nazwa = @nazwa)";
            return connection.Query<Student>(GetStudentsByYearAndLectureQuery, new { id_roku = id_roku, nazwa = nazwa }).ToList();
        }
        public static void EditLecture(string oldName, string newName, string oldTag, string newTag, int oldid_roku, int newid_roku)
        {
            string EditLectureQuery = "UPDATE lecture SET nazwa = @newName, tag = @newTag, rok = @newid_roku WHERE nazwa = @oldName AND tag = @oldTag AND rok = @oldid_roku";
            connection.ExecuteScalar(EditLectureQuery, new { oldName = oldName, newName = newName, oldTag = oldTag, newTag = newTag, oldid_roku = oldid_roku, newid_roku = newid_roku });
        }
        public static void EditYear(int oldid_roku, int newid_roku)
        {
            string EditYearQuery = "UPDATE year SET rok = @newid_roku WHERE rok = @oldid_roku";
            connection.ExecuteScalar(EditYearQuery, new { oldid_roku = oldid_roku, newid_roku = newid_roku });
        }
        public static List<Year> GetIdRokuByRok(int rok){
            string GetIdRokuByRokQuery = "SELECT * FROM year WHERE rok = @rok";
            return connection.Query<Year>(GetIdRokuByRokQuery, new { rok = rok }).ToList();
        }
        public static void EditStudent(int id, string oldImie, string newImie, string oldNazwisko, string newNazwisko, DateTime oldDataUrodzenia, DateTime newDataUrodzenia)
        {
            string EditStudentQuery = "UPDATE student SET imie = @newImie, nazwisko = @newNazwisko, data_urodzenia = @newDataUrodzenia WHERE id = @id AND imie = @oldImie AND nazwisko = @oldNazwisko AND data_urodzenia = @oldDataUrodzenia";
            connection.ExecuteScalar(EditStudentQuery, new { id = id, oldImie = oldImie, newImie = newImie, oldNazwisko = oldNazwisko, newNazwisko = newNazwisko, oldDataUrodzenia = oldDataUrodzenia, newDataUrodzenia = newDataUrodzenia });
        }
        public static int GetRokByRokId(int id_roku){
            string GetRokByRokIdQuery = "SELECT rok FROM year WHERE id = @id_roku";
            return connection.ExecuteScalar<int>(GetRokByRokIdQuery, new { id_roku = id_roku });
        }
        public static bool checkIfStudentHasThisYear(int id_student, int rok){
            string checkIfStudentHasThisYearQuery = "SELECT COUNT(*) FROM year WHERE id_studenta = @id_student AND rok = @rok";
            return connection.ExecuteScalar<int>(checkIfStudentHasThisYearQuery, new { id_student = id_student, rok = rok }) > 0 ? true : false;
        }
        public static void AddRokForStudent(int id_student, int rok){
            string AddRokForStudentQuery = "INSERT INTO year(rok, id_studenta) VALUES (@rok, @id_student)";
            connection.ExecuteScalar(AddRokForStudentQuery, new { rok = rok, id_student = id_student });
        }
        public static bool checkIfStudentHasThisLectureInYear(int id_student, string nazwa, int rok){
            string checkIfStudentHasThisLectureInYearQuery = "SELECT COUNT(*) FROM lecture WHERE id_studenta = @id_student AND nazwa = @nazwa AND id_roku = (SELECT id FROM year WHERE id_studenta = @id_student AND rok = @rok)";
            return connection.ExecuteScalar<int>(checkIfStudentHasThisLectureInYearQuery, new { id_student = id_student, nazwa = nazwa, rok = rok }) > 0 ? true : false;
        }
        public static string getTagByNazwa(string nazwa){
            string getTagByNazwaQuery = "SELECT tag FROM lecture WHERE nazwa = @nazwa";
            return connection.ExecuteScalar<string>(getTagByNazwaQuery, new { nazwa = nazwa });
        }
        public static void AddLectureForStudentInYear(int id_student, string nazwa, int id_roku){
            string AddLectureForStudentInYearQuery = "INSERT INTO lecture(tag, nazwa, id_roku, id_studenta) VALUES (@tag, @nazwa, @id_roku, @id_student)";
            connection.ExecuteScalar(AddLectureForStudentInYearQuery, new { tag = getTagByNazwa(nazwa), nazwa = nazwa, id_roku = id_roku, id_student = id_student });
        }
        public static void DeleteLectureByNazwa(string nazwa){
            string DeleteLectureByNazwaQuery = "DELETE FROM lecture WHERE nazwa = @nazwa";
            connection.ExecuteScalar(DeleteLectureByNazwaQuery, new { nazwa = nazwa });
        }
        public static void DeleteYearByRok(int rok){
            string DeleteYearByRokQuery = "DELETE FROM year WHERE rok = @rok";
            connection.ExecuteScalar(DeleteYearByRokQuery, new { rok = rok });
        }
        public static List<Student> GetStudentsByNazwaAndRok(string nazwa, int rok){
            string GetStudentsByNazwaAndRokQuery = "SELECT * FROM student WHERE id IN (SELECT id_studenta FROM lecture WHERE nazwa = @nazwa AND id_roku IN (SELECT id FROM year WHERE rok = @rok))";
            return connection.Query<Student>(GetStudentsByNazwaAndRokQuery, new { nazwa = nazwa, rok = rok }).ToList();
        }

    }



}

