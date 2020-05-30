using ORM;
//using System.Configuration;

namespace Test
{
    [Table(Name = "Fisica123")]
    public class Fisica : CType
    {
        [Table(Key = true, Name = "id")]
        public int Id { get; set; }

        [Table(Name = "Nome")]
        public string Nome { get; set; }

        [Table(Name = "Endereco")]
        public string Endereco { get; set; }

        [Table(Name = "Cpf")]
        public string Cpf { get; set; }

        //public override string ConnectionString => ConfigurationManager.ConnectionStrings["defaultConnection"].ConnectionString;

        public Fisica()
        {

        }

        public Fisica(int id, string nome, string endereco, string cpf)
        {
            this.Id = id;
            this.Nome = nome;
            this.Endereco = endereco;
            this.Cpf = cpf;
        }
    }
}
