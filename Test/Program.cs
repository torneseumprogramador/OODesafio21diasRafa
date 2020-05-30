using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            try
            {
                Fisica pessoa;
                
                pessoa = new Fisica(1, "Rafael", "Rua sem nome", "1223344");

                if (pessoa.Insert())
                {
                    Console.WriteLine(string.Format("Pessoa física {0} cadastrada com sucesso.", pessoa.Nome));
                }
                else
                {
                    Console.WriteLine(string.Format("Não foi possível salvar a pessoa física {0}.", pessoa.Nome));
                }
                
                new Fisica(5, "Vanessa", "Rua sem nome 5", "5678910").Update();                
                new Fisica(10, "João", "Rua sem nome 10", "11111111").Update();

                var list = new Fisica().GetAll<Fisica>();

                pessoa = new Fisica(1, "Rafael", "Rua sem nome", "1223344");
                var result = new Fisica().GetById(pessoa);

                new Fisica(22, "Rafael", "Rua sem nome", "1223344").Delete();

                pessoa = new Fisica
                {
                    Nome = "Vane",
                    Endereco = "Rua"
                };

                list = new Fisica().GetByFilter(pessoa, true);

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Ocorreu erro {0}.", ex.Message));
            }
        }
    }
}
