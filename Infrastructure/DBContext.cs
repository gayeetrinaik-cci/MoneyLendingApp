using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace Infrastructure
{
    public partial class ApplicationDBContext : DbContext
    {
        public virtual DbSet<Bankaccount> Bankaccounts { get; set; }

        public virtual DbSet<Company> Companies { get; set; }

        public virtual DbSet<Companystatus> Companystatuses { get; set; }

        public virtual DbSet<Loan> Loans { get; set; }

        public virtual DbSet<Loanconfirmationtoken> Loanconfirmationtokens { get; set; }

        public virtual DbSet<Loanstatus> Loanstatuses { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Paymentschedule> Paymentschedules { get; set; }

        public virtual DbSet<Payment> Payments { get; set; }

        public virtual DbSet<Penalty> Penalties { get; set; }


        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> contextOptions) :base(contextOptions)
        {
            
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bankaccount>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("bankaccount");

                entity.HasIndex(e => e.LoanId, "FK_BankAccount_LoanId");

                entity.Property(e => e.AccountNumber)
                    .HasMaxLength(100)
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");
                entity.Property(e => e.BankName)
                    .HasMaxLength(100)
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");

                entity.HasOne(d => d.Loan).WithMany(p => p.Bankaccounts)
                    .HasForeignKey(d => d.LoanId)
                    .HasConstraintName("FK_BankAccount_LoanId");
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("company");

                entity.HasIndex(e => e.ApprovedBy, "FK_Company_ApproveBy");

                entity.HasIndex(e => e.StatusId, "FK_Company_StatusId");

                entity.HasIndex(e => e.Name, "UQ_Company_Name").IsUnique();

                entity.Property(e => e.ApprovedOn).HasColumnType("datetime");
                entity.Property(e => e.Description)
                    .HasMaxLength(8000)
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");
                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");
                entity.Property(e => e.RegistrationDate).HasColumnType("datetime");
                entity.Property(e => e.RegistrationNumber)
                    .HasMaxLength(100)
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");

                entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.Companies)
                    .HasForeignKey(d => d.ApprovedBy)
                    .HasConstraintName("FK_Company_ApproveBy");

                entity.HasOne(d => d.Status).WithMany(p => p.Companies)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK_Company_StatusId");
            });

            modelBuilder.Entity<Companystatus>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("companystatus");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");
            });

            modelBuilder.Entity<Loan>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("loan");

                entity.HasIndex(e => e.CompanyId, "fk_Loan_CompanyId");

                entity.HasIndex(e => e.ProductId, "fk_Loan_ProductId");

                entity.HasIndex(e => e.StatusId, "fk_Loan_StatusId");

                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.AppovedOn).HasColumnType("datetime");
                entity.Property(e => e.GrantedOn).HasColumnType("datetime");
                entity.Property(e => e.LoanReason)
                    .HasMaxLength(1000)
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");

                entity.HasOne(d => d.Company).WithMany(p => p.Loans)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("fk_Loan_CompanyId");

                entity.HasOne(d => d.Product).WithMany(p => p.Loans)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("fk_Loan_ProductId");

                entity.HasOne(d => d.Status).WithMany(p => p.Loans)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("fk_Loan_StatusId");
            });

            modelBuilder.Entity<Loanconfirmationtoken>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("loanconfirmationtoken");

                entity.HasIndex(e => e.LoanId, "fk_LoanConfirmationToken_LoanId");

                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.ConfirmationCode)
                    .HasMaxLength(16)
                    .IsFixedLength();
                entity.Property(e => e.CreationTime).HasColumnType("datetime");
                entity.Property(e => e.ExpiryTime).HasColumnType("datetime");

                entity.HasOne(d => d.Loan).WithMany(p => p.Loanconfirmationtokens)
                    .HasForeignKey(d => d.LoanId)
                    .HasConstraintName("fk_LoanConfirmationToken_LoanId");
            });

            modelBuilder.Entity<Loanstatus>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("loanstatus");

                entity.HasIndex(e => e.StatusId, "UQ_LoanStatus_StatusId").IsUnique();

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("payment");

                entity.HasIndex(e => e.PaymentScheduleId, "FK_Payment_PaymentScheduleId");

                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.CummulativePaid).HasPrecision(18, 2);
                entity.Property(e => e.PaymentAmount).HasPrecision(18, 2);
                entity.Property(e => e.PaymentDate).HasColumnType("datetime");
                entity.Property(e => e.RemainingPayment).HasPrecision(18, 2);

                entity.HasOne(d => d.PaymentSchedule).WithMany(p => p.Payments)
                    .HasForeignKey(d => d.PaymentScheduleId)
                    .HasConstraintName("FK_Payment_PaymentScheduleId");
            });

            modelBuilder.Entity<Paymentschedule>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("paymentschedule");

                entity.HasIndex(e => e.LoanId, "FK_PaymentSchedule_LoanId");

                entity.Property(e => e.CummulativeAmount).HasPrecision(18, 2);
                entity.Property(e => e.Emi)
                    .HasPrecision(18, 2)
                    .HasColumnName("EMI");
                entity.Property(e => e.RemainingPayable).HasPrecision(18, 2);

                entity.HasOne(d => d.Loan).WithMany(p => p.Paymentschedules)
                    .HasForeignKey(d => d.LoanId)
                    .HasConstraintName("FK_PaymentSchedule_LoanId");
            });

            modelBuilder.Entity<Penalty>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("penalty");

                entity.HasIndex(e => e.PaymentId, "FK_Penalty_PaymentId");

                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Amount).HasPrecision(18, 2);

                entity.HasOne(d => d.Payment).WithMany(p => p.Penalties)
                    .HasForeignKey(d => d.PaymentId)
                    .HasConstraintName("FK_Penalty_PaymentId");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("product");

                entity.Property(e => e.IsPenaltyFixed).HasColumnType("bit(1)");
                entity.Property(e => e.Penalty).HasPrecision(18, 2);
                entity.Property(e => e.ProductType)
                    .HasMaxLength(100)
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");
                entity.Property(e => e.RateOfInterest).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("role");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("user");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");
                entity.Property(e => e.FirstName)
                    .HasMaxLength(100)
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");
                entity.Property(e => e.IsActive).HasColumnType("bit(1)");
                entity.Property(e => e.LastName)
                    .HasMaxLength(100)
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");

                entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "Userrole",
                        r => r.HasOne<Role>().WithMany()
                            .HasForeignKey("RoleId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK_UserRole_RoleId"),
                        l => l.HasOne<User>().WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK_UserRole_UserId"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId")
                                .HasName("PRIMARY")
                                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                            j.ToTable("userrole");
                            j.HasIndex(new[] { "RoleId" }, "FK_UserRole_RoleId");
                        });
            });

            //modelBuilder.Entity<Userrole>().HasKey(ur => new { ur.UserId, ur.RoleId });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
