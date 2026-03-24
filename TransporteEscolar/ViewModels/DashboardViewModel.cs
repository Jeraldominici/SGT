namespace TransporteEscolar.ViewModels
{
    public class DashboardViewModel
    {
        // ── Tarjetas resumen ──────────────────────────────────
        public int TotalUsuarios { get; set; }
        public int UsuariosActivos { get; set; }
        public int TotalAlumnos { get; set; }
        public int TotalAutobuses { get; set; }
        public int TotalChoferes { get; set; }
        public int AccesosHoy { get; set; }
        public int AusenciasHoy { get; set; }
        public int IncidenciasHoy { get; set; }

        // ── Gráfica: Accesos por día (últimos 14 días) ────────
        public List<string> AccesosFechas { get; set; } = new();
        public List<int> AccesosExitosos { get; set; } = new();
        public List<int> AccesosFallidos { get; set; } = new();

        // ── Gráfica: Asistencia por día (últimos 14 días) ─────
        public List<string> AsistFechas { get; set; } = new();  
        public List<int> AsistPresentes { get; set; } = new();
        public List<int> AsistAusentes { get; set; } = new();

        // ── Gráfica: Usuarios por rol ─────────────────────────
        public List<string> RolesNombres { get; set; } = new();
        public List<int> RolesCantidades { get; set; } = new();

        // ── Gráfica: Alumnos por autobús ──────────────────────
        public List<string> BusesFichas { get; set; } = new();
        public List<int> BusesAlumnos { get; set; } = new();

        // ── Gráfica: Incidencias por tipo ─────────────────────
        public List<string> IncTipos { get; set; } = new();
        public List<int> IncCantidades { get; set; } = new();
    }
}