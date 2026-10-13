using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Entities;

namespace Users.Infra.Data.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .ToTable("users");

            builder
                .HasKey(u => u.Id);

            builder
                .Property(u => u.Id)
                .HasColumnName("user_id")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder
                .Property(u => u.Name)
                .HasColumnName("user_name")
                .HasMaxLength(100)
                .IsRequired();

            builder
                .Property(u => u.Email)
                .HasColumnName("user_email")
                .HasMaxLength(50)
                .IsRequired();

            builder
                .Property(u => u.Password)
                .HasColumnName("user_password")
                .HasMaxLength(255)
                .IsRequired();

            builder
                .Property(u => u.Status)
                .HasColumnName("user_status")
                .HasMaxLength(1)
                .HasDefaultValue("A")
                .IsRequired();

            builder
                .Property(u => u.RoleId)
                .HasColumnName("role_id")
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .HasColumnName("user_createdate")
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();

            builder
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId);

            builder
                .HasData(new User(1, "Admin", "admin@fiapcloud.com.br", "$2a$12$20CoSGop/LM7eZ.6A4S68.B8zfIDD5oW8FQnfVCxV90BoSF498mbO", 1));
        }
    }
}
