namespace AuthenBackend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
        // เพิ่ม model ที่ต้องการ migrations
        public DbSet<User> Users => Set<User>();
    }
}