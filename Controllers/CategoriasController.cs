using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using APICatalogo.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace APICatalogo.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[Controller]")]
    [ApiController]
    [EnableCors("PermitirApiRequest")]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public CategoriasController(IUnitOfWork contexto, IConfiguration config, ILogger<CategoriasController> logger, IMapper mapper)
        {
            _uof = contexto;
            _configuration = config;
            _logger = logger;
            _mapper = mapper;
        }

        //[HttpGet("autor")]
        //public string GetAutor()
        //{
        //    var autor = _configuration["autor"];
        //    var conexao = _configuration["ConnectionStrings:DefaultConnection"];
        //    return $"Autor: {autor}, Conexão: {conexao}";
        //}

        //[HttpGet("saudacao/{nome}")]
        //public ActionResult<string> GetSaudacao([FromServices] IMeuServico meuServico, string nome)
        //{
        //    return meuServico.Saudacao(nome);
        //}

        [HttpGet]
        public async Task<ActionResult<CategoriaDTO>> Get([FromQuery] CategoriasParameters categoriasParameters)
        {
            try
            {
                var categorias = await _uof.CategoriaRepository.GetCategoriasPaginas(categoriasParameters);

                var metadata = new
                {
                    categorias.TotalCount,
                    categorias.PageSize,
                    categorias.CurrentPage,
                    categorias.TotalPages,
                    categorias.HasNext,
                    categorias.HasPrevious
                };

                Response.Headers.Add("X-pagination", JsonConvert.SerializeObject(metadata));

                var categoriasDTO = _mapper.Map<List<CategoriaDTO>>(categorias);
                return Ok(categoriasDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar obter as categorias no banco de dados!");
            }

        }

        [HttpGet("produtos")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasProdutos()
        {
            var categorias = await _uof.CategoriaRepository.GetCategoriasProdutos();
            var categoriasDTO = _mapper.Map<List<CategoriaDTO>>(categorias);
            return Ok(categoriasDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaDTO>> GetForId(int id)
        {
            try
            {
                var categoria = await _uof.CategoriaRepository.GetById(c => c.CategoriaId == id);
                if (categoria == null)
                {
                    return NotFound($"A Categoria com id={id} não foi encontrada!");
                }
                var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);
                return Ok(categoriaDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar obter a categoria no banco de dados!");
            }

        }

        [HttpGet("produtos/{id}")]
        public async Task<ActionResult<CategoriaDTO>> GetCategoriasProdutosForId(int id)
        {
            var categoria = await _uof.CategoriaRepository.Get().Where(c => c.CategoriaId == id).Include(x => x.Produtos).ToListAsync();
            if (categoria == null)
            {
                return NotFound();
            }
            var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);
            return Ok(categoriaDTO);
        }

        [HttpPost]
        public async Task<ActionResult<CategoriaDTO>> Post([FromBody] CategoriaDTO categoriaDto)
        {
            var categoria = _mapper.Map<Categoria>(categoriaDto);

            _uof.CategoriaRepository.Add(categoria);
            await _uof.Commit();

            var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);
            return Created("Categoria adicionada!", categoriaDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CategoriaDTO>> Put(int id, [FromBody] CategoriaDTO categoriaDto)
        {
            if (id != categoriaDto.CategoriaId)
            {
                return BadRequest("Id da categoria inválida!");
            }

            var categoria = _mapper.Map<Categoria>(categoriaDto);

            _uof.CategoriaRepository.Update(categoria);
            await _uof.Commit();
            return Ok("Categoria modificada com sucesso!");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<CategoriaDTO>> Delete(int id)
        {
            var categoria = await _uof.CategoriaRepository.GetById(c => c.CategoriaId == id);

            if (categoria == null)
            {
                return NotFound("Categoria não encontrada!");
            }
            else
            {
                _uof.CategoriaRepository.Delete(categoria);
                await _uof.Commit();

                var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);
                return Ok(categoriaDTO);
            }

        }

    }
}
