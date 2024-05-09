using Microsoft.AspNetCore.Mvc;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TarefaController : ControllerBase
    {
        private readonly OrganizadorContext _context;

        public TarefaController(OrganizadorContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            // Busca a tarefa no banco utilizando o EF
            var tarefa = _context.Tarefas.Find(id);

            // Valida o tipo de retorno. Se não encontrar a tarefa, retornar NotFound
            if (tarefa == null)
                return NotFound();

            // caso contrário retornar OK com a tarefa encontrada
            return Ok(tarefa);
        }

        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        {
            // Busca todas as tarefas no banco utilizando o EF
            var tarefas = _context.Tarefas.ToList();

            return Ok(tarefas);
        }

        [HttpGet("ObterPorTitulo")]
        public IActionResult ObterPorTitulo(string titulo)
        {
            // Executa tratamento no parâmetro recebido para case sensitive
            titulo = titulo.ToLower();

            // Busca  as tarefas no banco utilizando o EF, que contenha o titulo recebido por parâmetro
            var tarefas = _context.Tarefas.Where(t => t.Titulo.Contains(titulo));

            return Ok(tarefas);
        }

        [HttpGet("ObterPorData")]
        public IActionResult ObterPorData(DateTime data)
        {
            var tarefa = _context.Tarefas.Where(x => x.Data.Date == data.Date);
            return Ok(tarefa);
        }

        [HttpGet("ObterPorStatus")]
        public IActionResult ObterPorStatus(EnumStatusTarefa status)
        {
            // Busca  as tarefas no banco utilizando o EF, que contenha o status recebido por parâmetro
            var tarefa = _context.Tarefas.Where(x => x.Status == status);
            return Ok(tarefa);
        }

        [HttpPost]
        public IActionResult Criar(Tarefa tarefa)
        {
            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            // Adiciona a tarefa recebida no EF e salvar as mudanças (save changes)
            _context.Tarefas.Add(tarefa);
            _context.SaveChanges();

            return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
        }

        [HttpPost("CriarLote")]
        public IActionResult CriarLote(List<Tarefa> tarefas)
        {
            // Verifica se a lista de tarefas não está vazia
            if (tarefas == null || tarefas.Count == 0)
                return BadRequest(new { Erro = "A lista de tarefas está vazia" });

            var tarefasCriadas = new List<Tarefa>();

            foreach (var tarefa in tarefas)
            {
                if (tarefa.Data == DateTime.MinValue)
                    return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

                // Adiciona a tarefa recebida no EF
                _context.Tarefas.Add(tarefa);

                // Adiciona a tarefa à lista de tarefas criadas
                tarefasCriadas.Add(tarefa);
            }

            // Salva todas as mudanças de uma vez
            _context.SaveChanges();

            return Ok(new { Mensagem = "Tarefas criadas com sucesso", TarefasCriadas = tarefasCriadas });
        }


        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, Tarefa tarefa)
        {
            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound();

            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            // Atualiza as informações da variável tarefaBanco com a tarefa recebida via parâmetro
            tarefaBanco.Titulo = tarefa.Titulo;
            tarefaBanco.Status = tarefa.Status;
            tarefaBanco.Descricao = tarefa.Descricao;
            tarefaBanco.Data = tarefa.Data;

            _context.Tarefas.Update(tarefaBanco);
            _context.SaveChanges();

            // Atualiza a variável tarefaBanco no EF e salvar as mudanças (save changes)
            return Ok(tarefaBanco);
        }

        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound();

            // Remove a tarefa encontrada através do EF e salvar as mudanças (save changes)
            _context.Tarefas.Remove(tarefaBanco);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
