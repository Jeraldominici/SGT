using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using TransporteEscolar.ViewModels;

namespace TransporteEscolar.Services
{
    public class ExportService
    {
        // ════════════════════════════════════════════════════
        //  EXPORTAR A EXCEL
        // ════════════════════════════════════════════════════
        public byte[] ExportarExcel(ReporteViewModel modelo)
        {
            using var wb = new XLWorkbook();

            // ── Hoja 1: Resumen ──────────────────────────────
            var wsResumen = wb.Worksheets.Add("Resumen General");

            // ── Logo en Excel ─────────────────────────────────────
            try
            {
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Logo.png");
                if (System.IO.File.Exists(logoPath))
                {
                    var imagen = wsResumen.AddPicture(logoPath);
                    imagen.MoveTo(wsResumen.Cell("B1"));
                    imagen.Scale(0.18);  // ajusta el tamaño según tu logo
                }
            }
            catch { /* Si no carga la imagen, continúa sin ella */ }

            EstiloEncabezado(wsResumen, "REPORTE DE TRANSPORTE ESCOLAR", 1, 4);
            EstiloEncabezado(wsResumen, $"Período: {modelo.FechaDesde:dd/MM/yyyy} — {modelo.FechaHasta:dd/MM/yyyy}", 2, 4);

            

            int fila = 4;
            AgregarFilaTitulo(wsResumen, fila++, "RESUMEN GENERAL", 4);
            AgregarFila(wsResumen, fila++, "Total Usuarios", modelo.TotalUsuarios.ToString());
            AgregarFila(wsResumen, fila++, "Usuarios Activos", modelo.UsuariosActivos.ToString());
            AgregarFila(wsResumen, fila++, "Usuarios Bloqueados", modelo.UsuariosBloqueados.ToString());
            AgregarFila(wsResumen, fila++, "Total Autobuses", modelo.TotalAutobuses.ToString());
            AgregarFila(wsResumen, fila++, "Autobuses Activos", modelo.AutobusesActivos.ToString());
            fila++;
            AgregarFilaTitulo(wsResumen, fila++, "ACCESOS EN EL PERÍODO", 4);
            AgregarFila(wsResumen, fila++, "Total Accesos", modelo.TotalAccesos.ToString());
            AgregarFila(wsResumen, fila++, "Accesos Exitosos", modelo.AccesosExitosos.ToString());
            AgregarFila(wsResumen, fila++, "Accesos Fallidos", modelo.AccesosFallidos.ToString());
            wsResumen.Columns().AdjustToContents();

            // ── Hoja 2: Accesos por usuario ──────────────────
            var wsAccesos = wb.Worksheets.Add("Accesos por Usuario");
            var headersAccesos = new[] { "Usuario", "Nombre Completo", "Rol",
                                         "Total Accesos", "Exitosos", "Fallidos", "Último Acceso" };
            AgregarEncabezadoTabla(wsAccesos, 1, headersAccesos);
            int r = 2;
            foreach (var a in modelo.AccesosPorUsuario)
            {
                wsAccesos.Cell(r, 1).Value = a.NombreUsuario;
                wsAccesos.Cell(r, 2).Value = a.NombreCompleto;
                wsAccesos.Cell(r, 3).Value = a.Rol;
                wsAccesos.Cell(r, 4).Value = a.TotalAccesos;
                wsAccesos.Cell(r, 5).Value = a.Exitosos;
                wsAccesos.Cell(r, 6).Value = a.Fallidos;
                wsAccesos.Cell(r, 7).Value = a.UltimoAcceso.HasValue
                    ? a.UltimoAcceso.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm") : "Nunca";
                r++;
            }
            wsAccesos.Columns().AdjustToContents();

            // ── Hoja 3: Intentos fallidos ────────────────────
            var wsFallidos = wb.Worksheets.Add("Intentos Fallidos");
            var headersFallidos = new[] { "Usuario", "Nombre Completo", "Total Fallidos",
                                          "Bloqueado", "Última IP" };
            AgregarEncabezadoTabla(wsFallidos, 1, headersFallidos);
            int f = 2;
            foreach (var x in modelo.FallidosPorUsuario)
            {
                wsFallidos.Cell(f, 1).Value = x.NombreUsuario;
                wsFallidos.Cell(f, 2).Value = x.NombreCompleto;
                wsFallidos.Cell(f, 3).Value = x.TotalFallidos;
                wsFallidos.Cell(f, 4).Value = x.Bloqueado ? "Sí" : "No";
                wsFallidos.Cell(f, 5).Value = x.UltimaIP ?? "—";
                f++;
            }
            wsFallidos.Columns().AdjustToContents();

            // ── Hoja 4: Actividad por fecha ──────────────────
            var wsActividad = wb.Worksheets.Add("Actividad por Fecha");
            var headersActividad = new[] { "Fecha", "Exitosos", "Fallidos", "Total" };
            AgregarEncabezadoTabla(wsActividad, 1, headersActividad);
            int d = 2;
            foreach (var act in modelo.ActividadPorFecha)
            {
                wsActividad.Cell(d, 1).Value = act.Fecha.ToString("dd/MM/yyyy");
                wsActividad.Cell(d, 2).Value = act.Exitosos;
                wsActividad.Cell(d, 3).Value = act.Fallidos;
                wsActividad.Cell(d, 4).Value = act.Total;
                d++;
            }
            wsActividad.Columns().AdjustToContents();

            // ── Hoja 5: Usuarios bloqueados ──────────────────
            var wsBloqueados = wb.Worksheets.Add("Usuarios Bloqueados");
            var headersBloqueados = new[] { "Usuario", "Nombre Completo", "Rol",
                                            "Email", "Intentos Fallidos" };
            AgregarEncabezadoTabla(wsBloqueados, 1, headersBloqueados);
            int b = 2;
            foreach (var u in modelo.UsuariosBloqueadosList)
            {
                wsBloqueados.Cell(b, 1).Value = u.NombreUsuario;
                wsBloqueados.Cell(b, 2).Value = u.NombreCompleto;
                wsBloqueados.Cell(b, 3).Value = u.Rol?.Nombre ?? "—";
                wsBloqueados.Cell(b, 4).Value = u.Email ?? "—";
                wsBloqueados.Cell(b, 5).Value = u.IntentosFallidos;
                b++;
            }
            wsBloqueados.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        // ════════════════════════════════════════════════════
        //  EXPORTAR A PDF
        // ════════════════════════════════════════════════════
        public byte[] ExportarPdf(ReporteViewModel modelo)
        {
            using var ms = new MemoryStream();
            var doc = new Document(PageSize.A4, 40, 40, 60, 40);
            PdfWriter.GetInstance(doc, ms);
            doc.Open();

            // Fuentes
            var fuenteTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, new BaseColor(15, 23, 42));
            var fuenteSubtitulo = FontFactory.GetFont(FontFactory.HELVETICA, 10, new BaseColor(100, 116, 139));
            var fuenteSeccion = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, new BaseColor(37, 99, 235));
            var fuenteNormal = FontFactory.GetFont(FontFactory.HELVETICA, 9, new BaseColor(0, 0, 0));
            var fuenteHeader = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, new BaseColor(255, 255, 255));
            var fuenteBold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, new BaseColor(0, 0, 0));

            var azul = new BaseColor(37, 99, 235);
            var gris = new BaseColor(248, 250, 252);
            var gris2 = new BaseColor(226, 232, 240);

            
            // ── Logo ─────────────────────────────────────────────
            try
            {
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Logo.png");
                if (System.IO.File.Exists(logoPath))
                {
                    var logo = iTextSharp.text.Image.GetInstance(logoPath);
                    logo.Alignment = Element.ALIGN_CENTER;
                    logo.ScaleToFit(120f, 120f);
                    logo.SpacingAfter = 12f;
                    doc.Add(logo);
                }
            }
            catch { /* Si no carga la imagen, continúa sin ella */ }

            // ── Título ────────────────────────────────────────────
            doc.Add(new Paragraph("Reporte de Transporte Escolar", fuenteTitulo)
            { Alignment = Element.ALIGN_CENTER, SpacingAfter = 4 });
            doc.Add(new Paragraph(
                $"Período: {modelo.FechaDesde:dd/MM/yyyy} — {modelo.FechaHasta:dd/MM/yyyy}",
                fuenteSubtitulo)
            { Alignment = Element.ALIGN_CENTER, SpacingAfter = 4 });
            doc.Add(new Paragraph(
                $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}",
                fuenteSubtitulo)
            { Alignment = Element.ALIGN_CENTER, SpacingAfter = 20 });

            // ── Resumen general ──────────────────────────────
            doc.Add(new Paragraph("Resumen General", fuenteSeccion) { SpacingAfter = 8 });

            var tablaResumen = new PdfPTable(4) { WidthPercentage = 100, SpacingAfter = 20 };
            tablaResumen.SetWidths(new float[] { 25, 25, 25, 25 });

            AgregarCeldaResumen(tablaResumen, "Total Usuarios", modelo.TotalUsuarios.ToString(), azul, fuenteNormal, fuenteBold);
            AgregarCeldaResumen(tablaResumen, "Usuarios Activos", modelo.UsuariosActivos.ToString(), new BaseColor(21, 128, 61), fuenteNormal, fuenteBold);
            AgregarCeldaResumen(tablaResumen, "Usuarios Bloqueados", modelo.UsuariosBloqueados.ToString(), new BaseColor(220, 38, 38), fuenteNormal, fuenteBold);
            AgregarCeldaResumen(tablaResumen, "Total Autobuses", modelo.TotalAutobuses.ToString(), new BaseColor(217, 119, 6), fuenteNormal, fuenteBold);
            doc.Add(tablaResumen);

            // ── Accesos por usuario ──────────────────────────
            doc.Add(new Paragraph("Accesos por Usuario", fuenteSeccion) { SpacingAfter = 8 });
            var tablaAccesos = new PdfPTable(6) { WidthPercentage = 100, SpacingAfter = 20 };
            tablaAccesos.SetWidths(new float[] { 18, 28, 14, 14, 14, 12 });

            foreach (var h in new[] { "Usuario", "Nombre", "Rol", "Total", "Exitosos", "Fallidos" })
                AgregarCeldaHeader(tablaAccesos, h, azul, fuenteHeader);

            foreach (var a in modelo.AccesosPorUsuario)
            {
                tablaAccesos.AddCell(CeldaNormal(a.NombreUsuario, fuenteNormal));
                tablaAccesos.AddCell(CeldaNormal(a.NombreCompleto, fuenteNormal));
                tablaAccesos.AddCell(CeldaNormal(a.Rol, fuenteNormal));
                tablaAccesos.AddCell(CeldaNormal(a.TotalAccesos.ToString(), fuenteBold));
                tablaAccesos.AddCell(CeldaNormal(a.Exitosos.ToString(), fuenteNormal));
                tablaAccesos.AddCell(CeldaNormal(a.Fallidos.ToString(), fuenteNormal));
            }
            doc.Add(tablaAccesos);

            // ── Intentos fallidos ────────────────────────────
            if (modelo.FallidosPorUsuario.Any())
            {
                doc.Add(new Paragraph("Intentos Fallidos por Usuario", fuenteSeccion) { SpacingAfter = 8 });
                var tablaFallidos = new PdfPTable(4) { WidthPercentage = 100, SpacingAfter = 20 };
                tablaFallidos.SetWidths(new float[] { 25, 35, 20, 20 });

                foreach (var h in new[] { "Usuario", "Nombre Completo", "Fallidos", "Bloqueado" })
                    AgregarCeldaHeader(tablaFallidos, h, new BaseColor(220, 38, 38), fuenteHeader);

                foreach (var x in modelo.FallidosPorUsuario)
                {
                    tablaFallidos.AddCell(CeldaNormal(x.NombreUsuario, fuenteNormal));
                    tablaFallidos.AddCell(CeldaNormal(x.NombreCompleto, fuenteNormal));
                    tablaFallidos.AddCell(CeldaNormal(x.TotalFallidos.ToString(), fuenteBold));
                    tablaFallidos.AddCell(CeldaNormal(x.Bloqueado ? "Sí" : "No", fuenteNormal));
                }
                doc.Add(tablaFallidos);
            }

            // ── Actividad por fecha ──────────────────────────
            doc.Add(new Paragraph("Actividad por Fecha", fuenteSeccion) { SpacingAfter = 8 });
            var tablaActividad = new PdfPTable(4) { WidthPercentage = 100, SpacingAfter = 20 };
            tablaActividad.SetWidths(new float[] { 30, 25, 25, 20 });

            foreach (var h in new[] { "Fecha", "Exitosos", "Fallidos", "Total" })
                AgregarCeldaHeader(tablaActividad, h, new BaseColor(21, 128, 61), fuenteHeader);

            foreach (var act in modelo.ActividadPorFecha)
            {
                tablaActividad.AddCell(CeldaNormal(act.Fecha.ToString("dd/MM/yyyy"), fuenteNormal));
                tablaActividad.AddCell(CeldaNormal(act.Exitosos.ToString(), fuenteNormal));
                tablaActividad.AddCell(CeldaNormal(act.Fallidos.ToString(), fuenteNormal));
                tablaActividad.AddCell(CeldaNormal(act.Total.ToString(), fuenteBold));
            }
            doc.Add(tablaActividad);

            // ── Usuarios bloqueados ──────────────────────────
            if (modelo.UsuariosBloqueadosList.Any())
            {
                doc.Add(new Paragraph("Usuarios Bloqueados", fuenteSeccion) { SpacingAfter = 8 });
                var tablaBloqueados = new PdfPTable(4) { WidthPercentage = 100, SpacingAfter = 20 };
                tablaBloqueados.SetWidths(new float[] { 25, 35, 20, 20 });

                foreach (var h in new[] { "Usuario", "Nombre Completo", "Rol", "Intentos" })
                    AgregarCeldaHeader(tablaBloqueados, h, new BaseColor(220, 38, 38), fuenteHeader);

                foreach (var u in modelo.UsuariosBloqueadosList)
                {
                    tablaBloqueados.AddCell(CeldaNormal(u.NombreUsuario, fuenteNormal));
                    tablaBloqueados.AddCell(CeldaNormal(u.NombreCompleto, fuenteNormal));
                    tablaBloqueados.AddCell(CeldaNormal(u.Rol?.Nombre ?? "—", fuenteNormal));
                    tablaBloqueados.AddCell(CeldaNormal(u.IntentosFallidos.ToString(), fuenteBold));
                }
                doc.Add(tablaBloqueados);
            }

            doc.Close();
            return ms.ToArray();
        }

        // ── Helpers Excel ────────────────────────────────────
        private void EstiloEncabezado(IXLWorksheet ws, string texto, int fila, int cols)
        {
            ws.Cell(fila, 1).Value = texto;
            ws.Range(fila, 1, fila, cols).Merge();
            ws.Cell(fila, 1).Style.Font.Bold = true;
            ws.Cell(fila, 1).Style.Font.FontSize = 13;
            ws.Cell(fila, 1).Style.Font.FontColor = XLColor.FromArgb(15, 23, 42);
        }

        private void AgregarFilaTitulo(IXLWorksheet ws, int fila, string texto, int cols)
        {
            ws.Cell(fila, 1).Value = texto;
            ws.Range(fila, 1, fila, cols).Merge();
            ws.Cell(fila, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(37, 99, 235);
            ws.Cell(fila, 1).Style.Font.FontColor = XLColor.White;
            ws.Cell(fila, 1).Style.Font.Bold = true;
            ws.Cell(fila, 1).Style.Font.FontSize = 10;
        }

        private void AgregarFila(IXLWorksheet ws, int fila, string etiqueta, string valor)
        {
            ws.Cell(fila, 1).Value = etiqueta;
            ws.Cell(fila, 2).Value = valor;
            ws.Cell(fila, 1).Style.Font.Bold = true;
            ws.Cell(fila, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(248, 250, 252);
        }

        private void AgregarEncabezadoTabla(IXLWorksheet ws, int fila, string[] headers)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(fila, i + 1).Value = headers[i];
                ws.Cell(fila, i + 1).Style.Fill.BackgroundColor = XLColor.FromArgb(37, 99, 235);
                ws.Cell(fila, i + 1).Style.Font.FontColor = XLColor.White;
                ws.Cell(fila, i + 1).Style.Font.Bold = true;
            }
        }

        // ── Helpers PDF ──────────────────────────────────────
        private void AgregarCeldaHeader(PdfPTable tabla, string texto,
            BaseColor color, iTextSharp.text.Font fuente)
        {
            var celda = new PdfPCell(new Phrase(texto, fuente))
            {
                BackgroundColor = color,
                Padding = 6,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            tabla.AddCell(celda);
        }

        private void AgregarCeldaResumen(PdfPTable tabla, string etiqueta,
            string valor, BaseColor color,
            iTextSharp.text.Font fuenteNormal, iTextSharp.text.Font fuenteBold)
        {
            var celda = new PdfPCell
            {
                BackgroundColor = new BaseColor(248, 250, 252),
                Padding = 10,
                Border = Rectangle.BOX,
                BorderColor = new BaseColor(226, 232, 240)
            };
            celda.AddElement(new Paragraph(valor,
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, color))
            { Alignment = Element.ALIGN_CENTER });
            celda.AddElement(new Paragraph(etiqueta, fuenteNormal)
            { Alignment = Element.ALIGN_CENTER });
            tabla.AddCell(celda);
        }

        private PdfPCell CeldaNormal(string texto, iTextSharp.text.Font fuente)
            => new PdfPCell(new Phrase(texto, fuente))
            {
                Padding = 5,
                Border = Rectangle.BOX,
                BorderColor = new BaseColor(226, 232, 240)
            };
    }
}