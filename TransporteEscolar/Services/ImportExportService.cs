using ClosedXML.Excel;
using System.Text.Json;
using TransporteEscolar.Models;
using TransporteEscolar.Repositories.Interfaces;

namespace TransporteEscolar.Services
{
    public class ImportExportService
    {
        private readonly IUsuarioRepository _repo;

        public ImportExportService(IUsuarioRepository repo)
            => _repo = repo;

        // ════════════════════════════════════════════════════
        //  EXPORTAR
        // ════════════════════════════════════════════════════

        public async Task<byte[]> ExportarAlumnosAsync()
        {
            var alumnos = await _repo.ObtenerTodosAlumnosAsync();
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Alumnos");

            var headers = new[] {
                "AlumnoId", "NombreCompleto", "FechaNacimiento", "GradoEscolar",
                "NombreTutor", "TelefonoEmergencia", "DireccionRecogida",
                "DireccionEntrega", "AutobusId", "Activo"
            };
            AgregarEncabezados(ws, headers);

            int fila = 2;
            foreach (var a in alumnos)
            {
                ws.Cell(fila, 1).Value = a.AlumnoId;
                ws.Cell(fila, 2).Value = a.NombreCompleto;
                ws.Cell(fila, 3).Value = a.FechaNacimiento?.ToString("yyyy-MM-dd") ?? "";
                ws.Cell(fila, 4).Value = a.GradoEscolar ?? "";
                ws.Cell(fila, 5).Value = a.NombreTutor ?? "";
                ws.Cell(fila, 6).Value = a.TelefonoEmergencia ?? "";
                ws.Cell(fila, 7).Value = a.DireccionRecogida ?? "";
                ws.Cell(fila, 8).Value = a.DireccionEntrega ?? "";
                ws.Cell(fila, 9).Value = a.AutobusId;
                ws.Cell(fila, 10).Value = a.Activo ? "Sí" : "No";
                fila++;
            }

            ws.Columns().AdjustToContents();
            return ToBytes(wb);
        }

        public async Task<byte[]> ExportarAutobusesAsync()
        {
            var buses = await _repo.ObtenerTodosAutobusesAsync();
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Autobuses");

            var headers = new[] { "AutobusId", "Ficha", "Placa", "Capacidad", "Activo" };
            AgregarEncabezados(ws, headers);

            int fila = 2;
            foreach (var b in buses)
            {
                ws.Cell(fila, 1).Value = b.AutobusId;
                ws.Cell(fila, 2).Value = b.Ficha;
                ws.Cell(fila, 3).Value = b.Placa ?? "";
                ws.Cell(fila, 4).Value = b.Capacidad;
                ws.Cell(fila, 5).Value = b.Activo ? "Sí" : "No";
                fila++;
            }

            ws.Columns().AdjustToContents();
            return ToBytes(wb);
        }

        public async Task<byte[]> ExportarChoferesAsync()
        {
            var choferes = await _repo.ObtenerChoferesAsync();
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Choferes");

            var headers = new[] {
                "ChoferId", "NombreCompleto", "Telefono", "DUI", "Licencia", "Activo"
            };
            AgregarEncabezados(ws, headers);

            int fila = 2;
            foreach (var c in choferes)
            {
                ws.Cell(fila, 1).Value = c.ChoferId;
                ws.Cell(fila, 2).Value = c.NombreCompleto;
                ws.Cell(fila, 3).Value = c.Telefono ?? "";
                ws.Cell(fila, 4).Value = c.DUI;
                ws.Cell(fila, 5).Value = c.Licencia;
                ws.Cell(fila, 6).Value = c.Activo ? "Sí" : "No";
                fila++;
            }

            ws.Columns().AdjustToContents();
            return ToBytes(wb);
        }

        public async Task<byte[]> ExportarAzafatasAsync()
        {
            var usuarios = await _repo.ObtenerTodosAsync();
            var azafatas = usuarios.Where(u => u.Rol?.Nombre == "Azafata").ToList();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Azafatas");

            var headers = new[] {
                "UsuarioId", "NombreUsuario", "NombreCompleto", "Email", "AutobusId", "Activo"
            };
            AgregarEncabezados(ws, headers);

            int fila = 2;
            foreach (var a in azafatas)
            {
                ws.Cell(fila, 1).Value = a.UsuarioId;
                ws.Cell(fila, 2).Value = a.NombreUsuario;
                ws.Cell(fila, 3).Value = a.NombreCompleto;
                ws.Cell(fila, 4).Value = a.Email ?? "";
                ws.Cell(fila, 5).Value = a.AutobusId?.ToString() ?? "";
                ws.Cell(fila, 6).Value = a.Activo ? "Sí" : "No";
                fila++;
            }

            ws.Columns().AdjustToContents();
            return ToBytes(wb);
        }

        // ════════════════════════════════════════════════════
        //  PLANTILLAS
        // ════════════════════════════════════════════════════

        public byte[] GenerarPlantilla(string entidad)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(entidad);

            string[] headers = entidad switch
            {
                "Alumnos" => new[] { "NombreCompleto", "FechaNacimiento(yyyy-MM-dd)",
                                       "GradoEscolar", "NombreTutor", "TelefonoEmergencia",
                                       "DireccionRecogida", "DireccionEntrega", "AutobusId" },
                "Autobuses" => new[] { "Ficha", "Placa", "Capacidad" },
                "Choferes" => new[] { "NombreCompleto", "Telefono", "DUI", "Licencia" },
                "Azafatas" => new[] { "NombreUsuario", "NombreCompleto", "Email",
                                       "Password", "AutobusId" },
                _ => Array.Empty<string>()
            };

            AgregarEncabezados(ws, headers);

            // Fila de ejemplo en gris
            var ejemplos = entidad switch
            {
                "Alumnos" => new[] { "Carlos López", "2015-03-10", "3° Primaria",
                                       "Ana López", "7801-1111", "Col. Las Flores #12",
                                       "Escuela Central", "1" },
                "Autobuses" => new[] { "B-001", "P-1234", "30" },
                "Choferes" => new[] { "Roberto Martínez", "7801-2345", "01234567-8", "L-001234" },
                "Azafatas" => new[] { "azafata02", "María Pérez", "maria@correo.com",
                                       "Clave123!", "1" },
                _ => Array.Empty<string>()
            };

            for (int i = 0; i < ejemplos.Length; i++)
            {
                ws.Cell(2, i + 1).Value = ejemplos[i];
                ws.Cell(2, i + 1).Style.Font.FontColor = XLColor.Gray;
                ws.Cell(2, i + 1).Style.Font.Italic = true;
            }

            ws.Row(2).Style.Fill.BackgroundColor = XLColor.FromArgb(248, 250, 252);
            ws.Columns().AdjustToContents();
            return ToBytes(wb);
        }

        // ════════════════════════════════════════════════════
        //  PREVIEW DE IMPORTACIÓN
        // ════════════════════════════════════════════════════

        public ViewModels.ImportPreviewViewModel GenerarPreview(
            IFormFile archivo, string entidad)
        {
            var preview = new ViewModels.ImportPreviewViewModel { Entidad = entidad };

            using var stream = archivo.OpenReadStream();
            using var wb = new XLWorkbook(stream);
            var ws = wb.Worksheets.First();
            var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
            var lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 1;

            // Encabezados
            for (int c = 1; c <= lastCol; c++)
                preview.Columnas.Add(ws.Cell(1, c).GetString());

            // Filas de datos (saltamos la fila de ejemplo si es gris)
            for (int r = 2; r <= lastRow; r++)
            {
                var fila = new List<string>();
                bool esEjemplo = ws.Cell(r, 1).Style.Font.Italic;
                if (esEjemplo) continue;

                for (int c = 1; c <= lastCol; c++)
                    fila.Add(ws.Cell(r, c).GetString().Trim());

                if (fila.All(string.IsNullOrWhiteSpace)) continue;

                // Validación básica
                var error = ValidarFila(fila, entidad, r);
                if (error != null)
                    preview.Errores.Add(error);

                preview.Filas.Add(fila);
            }

            preview.DatosJson = JsonSerializer.Serialize(
                new { entidad, filas = preview.Filas });

            return preview;
        }

        // ════════════════════════════════════════════════════
        //  IMPORTAR (confirmar tras preview)
        // ════════════════════════════════════════════════════

        public async Task<(int creados, int actualizados, List<string> errores)>
            ImportarAsync(string entidad, List<List<string>> filas)
        {
            int creados = 0, actualizados = 0;
            var errores = new List<string>();

            foreach (var (fila, idx) in filas.Select((f, i) => (f, i + 2)))
            {
                try
                {
                    switch (entidad)
                    {
                        case "Alumnos":
                            await ImportarAlumnoAsync(fila);
                            creados++;
                            break;
                        case "Autobuses":
                            await ImportarAutobusAsync(fila);
                            creados++;
                            break;
                        case "Choferes":
                            await ImportarChoferAsync(fila);
                            creados++;
                            break;
                        case "Azafatas":
                            await ImportarAzafataAsync(fila);
                            creados++;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    errores.Add($"Fila {idx}: {ex.Message}");
                }
            }

            return (creados, actualizados, errores);
        }

        // ── Importar entidades individuales ──────────────────

        private async Task ImportarAlumnoAsync(List<string> f)
        {
            if (!int.TryParse(f.ElementAtOrDefault(7), out var autobusId))
                throw new Exception("AutobusId inválido.");

            await _repo.CrearAlumnoAsync(new Alumno
            {
                NombreCompleto = f[0],
                FechaNacimiento = DateOnly.TryParse(f.ElementAtOrDefault(1), out var fn) ? fn : null,
                GradoEscolar = f.ElementAtOrDefault(2),
                NombreTutor = f.ElementAtOrDefault(3),
                TelefonoEmergencia = f.ElementAtOrDefault(4),
                DireccionRecogida = f.ElementAtOrDefault(5),
                DireccionEntrega = f.ElementAtOrDefault(6),
                AutobusId = autobusId,
                Activo = true,
                FechaAlta = DateTime.UtcNow
            });
        }

        private async Task ImportarAutobusAsync(List<string> f)
        {
            if (!int.TryParse(f.ElementAtOrDefault(2), out var capacidad))
                throw new Exception("Capacidad inválida.");

            if (await _repo.ExisteFichaAsync(f[0]))
                throw new Exception($"La ficha '{f[0]}' ya existe.");

            await _repo.CrearAutobusAsync(new Autobus
            {
                Ficha = f[0],
                Placa = f.ElementAtOrDefault(1),
                Capacidad = capacidad,
                Activo = true
            });
        }

        private async Task ImportarChoferAsync(List<string> f)
        {
            if (await _repo.ExisteDUIAsync(f[2]))
                throw new Exception($"El DUI '{f[2]}' ya existe.");

            await _repo.CrearChoferAsync(new Chofer
            {
                NombreCompleto = f[0],
                Telefono = f.ElementAtOrDefault(1),
                DUI = f[2],
                Licencia = f[3],
                Activo = true,
                FechaAlta = DateTime.UtcNow
            });
        }

        private async Task ImportarAzafataAsync(List<string> f)
        {
            if (await _repo.ExisteNombreUsuarioAsync(f[0]))
                throw new Exception($"El usuario '{f[0]}' ya existe.");

            if (!int.TryParse(f.ElementAtOrDefault(4), out var autobusId))
                throw new Exception("AutobusId inválido.");

            var roles = await _repo.ObtenerRolesAsync();
            var rolAzafata = roles.FirstOrDefault(r => r.Nombre == "Azafata")
                ?? throw new Exception("Rol Azafata no encontrado.");

            await _repo.CrearAsync(new Models.Usuario
            {
                NombreUsuario = f[0],
                NombreCompleto = f[1],
                Email = f.ElementAtOrDefault(2),
                PasswordHash = Helpers.PasswordHelper.HashPassword(
                    f.ElementAtOrDefault(3) ?? "Temporal123!"),
                RolId = rolAzafata.RolId,
                AutobusId = autobusId,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            });
        }

        // ── Validación básica ─────────────────────────────────

        private string? ValidarFila(List<string> fila, string entidad, int numFila)
        {
            return entidad switch
            {
                "Alumnos" when string.IsNullOrWhiteSpace(fila.ElementAtOrDefault(0))
                    => $"Fila {numFila}: NombreCompleto es obligatorio.",
                "Alumnos" when !int.TryParse(fila.ElementAtOrDefault(7), out _)
                    => $"Fila {numFila}: AutobusId debe ser un número.",
                "Autobuses" when string.IsNullOrWhiteSpace(fila.ElementAtOrDefault(0))
                    => $"Fila {numFila}: Ficha es obligatoria.",
                "Autobuses" when !int.TryParse(fila.ElementAtOrDefault(2), out _)
                    => $"Fila {numFila}: Capacidad debe ser un número.",
                "Choferes" when string.IsNullOrWhiteSpace(fila.ElementAtOrDefault(0))
                    => $"Fila {numFila}: NombreCompleto es obligatorio.",
                "Choferes" when string.IsNullOrWhiteSpace(fila.ElementAtOrDefault(2))
                    => $"Fila {numFila}: DUI es obligatorio.",
                "Azafatas" when string.IsNullOrWhiteSpace(fila.ElementAtOrDefault(0))
                    => $"Fila {numFila}: NombreUsuario es obligatorio.",
                _ => null
            };
        }

        // ── Helpers ───────────────────────────────────────────

        private void AgregarEncabezados(IXLWorksheet ws, string[] headers)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(1, i + 1).Value = headers[i];
                ws.Cell(1, i + 1).Style.Font.Bold = true;
                ws.Cell(1, i + 1).Style.Fill.BackgroundColor =
                    XLColor.FromArgb(37, 99, 235);
                ws.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
            }
        }

        private byte[] ToBytes(XLWorkbook wb)
        {
            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }
    }
}