using ApiMovies.Common.Applications.Interfaces;
using ApiMovies.Common.Dtos;
using ApiMovies.Common.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiMovies.Controllers
{
    [ApiController]
    //[Route("api/[controller]")]//Una opción
    [Route("api/categorias")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepositorio _categoriaRepositorio;
        private readonly IMapper _mapper;

        public CategoriasController(ICategoriaRepositorio categoriaRepositorio, IMapper mapper)
        {
            _categoriaRepositorio = categoriaRepositorio;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        [ResponseCache(Duration = 20)]
        //[ResponseCache(CacheProfileName = "PorDefecto20Segundos")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCategorias()
        {
            var listaCategorias = await _categoriaRepositorio.GetCategoriasAsync();

            if (!listaCategorias.IsSuccess)
            {
                return BadRequest(listaCategorias.ErrorMessage);
            }

            var listaCategoriasDto = new List<CategoriaDto>();

            foreach (var lista in listaCategorias.MyCollection)
            {
                listaCategoriasDto.Add(_mapper.Map<CategoriaDto>(lista));
            }
            return Ok(listaCategoriasDto);
        }

        [AllowAnonymous]
        [HttpGet("{categoriaId:int}", Name = "GetCategoria")]
        //[ResponseCache(CacheProfileName = "PorDefecto20Segundos")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCategoria(int categoriaId)
        {
            var itemCategoria = await _categoriaRepositorio.GetCategoriaAsync(categoriaId);

            if (!itemCategoria.IsSuccess)
            {
                return NotFound(itemCategoria.ErrorMessage);
            }

            var itemCategoriaDto = _mapper.Map<CategoriaDto>(itemCategoria.DirectObject);
            return Ok(itemCategoriaDto);
        }

        //[Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CategoriaDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CrearCategoria([FromBody] CrearCategoriaDto crearCategoriaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (crearCategoriaDto == null)
            {
                return BadRequest(ModelState);
            }
            var DtoResult = await _categoriaRepositorio.ExisteCategoriaAsync(crearCategoriaDto.Nombre);
            if (DtoResult.IsSuccess)
            {
                ModelState.AddModelError("", "La categoría ya existe");
                return StatusCode(404, ModelState);
            }

            var categoria = _mapper.Map<Categoria>(crearCategoriaDto);

            var created = await _categoriaRepositorio.CrearCategoriaAsync(categoria);

            if (!created.IsSuccess)
            {
                ModelState.AddModelError("", $"Algo salió mal guardando el registro{categoria.Nombre}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetCategoria", new { categoriaId = categoria.Id }, categoria);
        }

        //[Authorize(Roles = "admin")]
        [HttpPatch("{categoriaId:int}", Name = "ActualizarPatchCategoria")]
        [ProducesResponseType(201, Type = typeof(CategoriaDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ActualizarPatchCategoria(int categoriaId, [FromBody] CategoriaDto categoriaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (categoriaDto == null || categoriaId != categoriaDto.Id)
            {
                return BadRequest(ModelState);
            }

            var categoria = _mapper.Map<Categoria>(categoriaDto);
            var updated = await _categoriaRepositorio.ActualizarCategoriaAsync(categoria);
            if (!updated.IsSuccess)
            {
                ModelState.AddModelError("", $"Algo salió mal actualizando el registro{categoria.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        //[Authorize(Roles = "admin")]
        [HttpDelete("{categoriaId:int}", Name = "BorrarCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BorrarCategoria(int categoriaId)
        {
            var existe = await _categoriaRepositorio.ExisteCategoriaAsync(categoriaId);
            if (!existe.IsSuccess)
            {
                return NotFound();
            }

            var categoria = await _categoriaRepositorio.GetCategoriaAsync(categoriaId);

            if (!categoria.IsSuccess)
            {
                return BadRequest(categoria.ErrorMessage);
            }
            var deleted = await _categoriaRepositorio.BorrarCategoriaAsync(categoria.DirectObject);
            if (!deleted.IsSuccess)
            {
                ModelState.AddModelError("", $"Algo salió mal borrando el registro{categoria.DirectObject.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

    }
}
