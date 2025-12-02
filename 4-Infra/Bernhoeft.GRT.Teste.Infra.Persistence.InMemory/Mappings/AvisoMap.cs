using Bernhoeft.GRT.Teste.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bernhoeft.GRT.Teste.Infra.Persistence.InMemory.Mappings
{
    /// <summary>
    /// Mapeamento da entidade AvisoEntity para o Entity Framework.
    /// </summary>
    public partial class AvisoMap : IEntityTypeConfiguration<AvisoEntity>
    {
        public void Configure(EntityTypeBuilder<AvisoEntity> builder)
        {
            builder.ToTable("Aviso", "dbo");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"id").HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Ativo).HasColumnName(@"ativo").HasColumnType("bit").IsRequired();
            builder.Property(x => x.Titulo).HasColumnName(@"titulo").HasColumnType("varchar(50)").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(x => x.Mensagem).HasColumnName(@"mensagem").HasColumnType("text(2147483647)").IsRequired().IsUnicode(false).HasMaxLength(2147483647);
            builder.Property(x => x.DataCriacao).HasColumnName(@"data_criacao").HasColumnType("datetime2").IsRequired();
            builder.Property(x => x.DataEdicao).HasColumnName(@"data_edicao").HasColumnType("datetime2").IsRequired(false);

            InitializePartial(builder);
        }

        partial void InitializePartial(EntityTypeBuilder<AvisoEntity> builder);
    }
}
