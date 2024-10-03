using Microsoft.EntityFrameworkCore;

namespace LoadDataUser
{
    //доп задание в Test
     class Program
    {
        static async Task Main(string[] args)
        {
            var tasks = new List<Task>();
            Test test = new Test();
            

            tasks.Add(test.GetArray(new string[] { "Artem", "Grigoriy", "Anna" }));
            tasks.Add(test.GetArray(new string[] { "Elena", "Pavel", "Elena" })); 
            tasks.Add(test.GetArray(new string[] { "Mikhail", "Ivan", "Olga" }));


            await InisializeAsync();
            try
            {
                User currentUser = await LoadUserDataAsync();
                if (currentUser != null)
                {
                    
                        Console.WriteLine($"ID {currentUser.Id}, Name: {currentUser.Name}, Email: {currentUser.Email}");
                    
                }
            }
            catch(OperationCanceledException)
            {
                Console.WriteLine("Запрос к серверу отменен, превышено время ожидания");
            }
        }




        static async Task<User?> LoadUserDataAsync()
        {
            CancellationTokenSource canceltokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            CancellationToken canceltoken = canceltokenSource.Token;
            Task<User> loadTask = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(3));
                using (Applicationcontext context = new Applicationcontext())
                {
                    return  context.Users.FirstOrDefault()!;
                }
            });

            if (await Task.WhenAny(loadTask, Task.Delay(TimeSpan.FromSeconds(10), canceltoken)) == loadTask)
            {
                return await loadTask;
            }
            else
            {
                throw new OperationCanceledException("Запрос к серверу был отменен");
            }
        }

        static async Task InisializeAsync()
        {
            using (Applicationcontext db = new Applicationcontext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Users.AddRange
                    (
                    new User[]
                    {
                    new User { Name = "Artem", Email = "something@gmail.com" },
                    new User { Name = "Andrei", Email = "always@gmail.com" }
                    });
                db.SaveChanges();
            }

        }
    }





    public class Applicationcontext : DbContext
    {
        public DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-TBASQVJ;Database=testdb;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }



    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
