using Newtonsoft.Json;
using SistemaLogistica.Models;

namespace SistemaLogistica.Data
{
    public class EnvioRepository
    {
        private readonly string _rutaArchivo;
        private List<Envio> _envios;

        public EnvioRepository(string rutaArchivo = "Data/envios.json")
        {
            _rutaArchivo = rutaArchivo;
            _envios = new List<Envio>();

            var directorio = Path.GetDirectoryName(_rutaArchivo);
            if (!string.IsNullOrEmpty(directorio) && !Directory.Exists(directorio))
            {
                Directory.CreateDirectory(directorio);
            }

            CargarDatos();
        }

        private void CargarDatos()
        {
            if (File.Exists(_rutaArchivo))
            {
                var json = File.ReadAllText(_rutaArchivo);
                _envios = JsonConvert.DeserializeObject<List<Envio>>(json) ?? new List<Envio>();
            }
        }

        private void GuardarDatos()
        {
            var json = JsonConvert.SerializeObject(_envios, Formatting.Indented);
            File.WriteAllText(_rutaArchivo, json);
        }

        public int ObtenerSiguienteId()
        {
            return _envios.Count > 0 ? _envios.Max(e => e.Id) + 1 : 101;
        }

        public void Agregar(Envio envio)
        {
            _envios.Add(envio);
            GuardarDatos();
        }

        public List<Envio> ObtenerTodos()
        {
            return _envios;
        }

        public Envio? BuscarPorId(int id)
        {
            return _envios.FirstOrDefault(e => e.Id == id);
        }

        public void Actualizar(Envio envio)
        {
            var existente = BuscarPorId(envio.Id);
            if (existente != null)
            {
                var index = _envios.IndexOf(existente);
                _envios[index] = envio;
                GuardarDatos();
            }
        }

        public void Eliminar(int id)
        {
            var envio = BuscarPorId(id);
            if (envio != null)
            {
                _envios.Remove(envio);
                GuardarDatos();
            }
        }
    }
}