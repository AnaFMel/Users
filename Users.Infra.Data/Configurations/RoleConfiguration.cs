using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Entities;
using Users.Domain.Enums;

namespace Users.Infra.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder
                .ToTable("roles");

            builder
                .HasKey(p => p.Id);

            builder
                .Property(p => p.Id)
                .HasColumnName("role_id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder
                .Property(p => p.Name)
                .HasColumnName("role_name")
                .HasColumnType("varchar(5)")
                .IsRequired();

            builder
                .Ignore(p => p.Status);

            builder
                .Property(p => p.CreatedAt)
                .HasColumnName("role_createdat")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("current_timestamp()")
                .IsRequired();

            builder
                .HasData(
                    new Role(1, nameof(Policy.Admin)),
                    new Role(2, nameof(Policy.User))
                );
        }
    }
}
