using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Entities;
using Users.Domain.Services;

namespace Users.Infra.Data.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        private readonly PasswordService _passwordService;

        public UserConfiguration(PasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .ToTable("users");

            builder
                .HasKey(u => u.Id);

            builder
                .Property(u => u.Id)
                .HasColumnName("user_id")
                .HasColumnType("int") 
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder
                .Property(u => u.Name)
                .HasColumnName("user_name")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder
                .Property(u => u.Email)
                .HasColumnName("user_email")
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder
                .Property(u => u.Password)
                .HasColumnName("user_password")
                .HasColumnType("varchar(255)")
                .IsRequired();

            builder
                .Property(u => u.Status)
                .HasColumnName("user_status")
                .HasColumnType("char(1)")
                .HasDefaultValue("A")
                .IsRequired();

            builder
                .Property(u => u.RoleId)
                .HasColumnName("role_id")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(u => u.CreatedAt)
                .HasColumnName("user_createdat")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("current_timestamp()")
                .IsRequired();

            builder
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId);

            builder
                .HasData(new User(1, "Admin", "admin@fiapcloud.com.br", _passwordService.CreateHash("admin"), 1));
        }
    }
}
