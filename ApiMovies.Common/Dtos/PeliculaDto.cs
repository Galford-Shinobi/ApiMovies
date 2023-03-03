using System.ComponentModel.DataAnnotations;

namespace ApiMovies.Common.Dtos
{
    public class PeliculaDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(150, ErrorMessage = "El campo {0} debe tener una longitud máxima de {1} caracteres")]
        public string Nombre { get; set; }
        public string RutaImagen { get; set; }
        public byte[] ByteImagen { get; set; }
        [Required(ErrorMessage = "La descripción es obligatoria")]
        [MaxLength(150, ErrorMessage = "El campo {0} debe tener una longitud máxima de {1} caracteres")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "La duración es obligatoria")]
        public int Duracion { get; set; }

        public enum TipoClasificacion { Siete, Trece, Dieciseis, Dieciocho }

        public TipoClasificacion Clasificacion { get; set; }
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Categoria")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debes de ingresar un valor mayor a cero en la {0}.")]
        public int categoriaId { get; set; }

    }
}
