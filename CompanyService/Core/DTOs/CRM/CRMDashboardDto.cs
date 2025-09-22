namespace CompanyService.Core.DTOs.CRM
{
    /// <summary>
    /// DTO para el dashboard de CRM con métricas y estadísticas
    /// </summary>
    public class CRMDashboardDto
    {
        /// <summary>
        /// Estadísticas de leads
        /// </summary>
        public LeadStatistics LeadStats { get; set; } = new();

        /// <summary>
        /// Estadísticas de oportunidades
        /// </summary>
        public OpportunityStatistics OpportunityStats { get; set; } = new();

        /// <summary>
        /// Estadísticas de campañas
        /// </summary>
        public CampaignStatistics CampaignStats { get; set; } = new();

        /// <summary>
        /// Leads recientes
        /// </summary>
        public List<RecentLeadDto> RecentLeads { get; set; } = new();

        /// <summary>
        /// Oportunidades próximas a cerrar
        /// </summary>
        public List<UpcomingOpportunityDto> UpcomingOpportunities { get; set; } = new();

        /// <summary>
        /// Actividades recientes
        /// </summary>
        public List<RecentActivityDto> RecentActivities { get; set; } = new();
    }

    /// <summary>
    /// Estadísticas de leads
    /// </summary>
    public class LeadStatistics
    {
        /// <summary>
        /// Total de leads
        /// </summary>
        public int TotalLeads { get; set; }

        /// <summary>
        /// Leads nuevos este mes
        /// </summary>
        public int NewLeadsThisMonth { get; set; }

        /// <summary>
        /// Leads convertidos este mes
        /// </summary>
        public int ConvertedLeadsThisMonth { get; set; }

        /// <summary>
        /// Tasa de conversión
        /// </summary>
        public decimal ConversionRate { get; set; }
    }

    /// <summary>
    /// Estadísticas de oportunidades
    /// </summary>
    public class OpportunityStatistics
    {
        /// <summary>
        /// Total de oportunidades
        /// </summary>
        public int TotalOpportunities { get; set; }

        /// <summary>
        /// Oportunidades abiertas
        /// </summary>
        public int OpenOpportunities { get; set; }

        /// <summary>
        /// Valor total de oportunidades abiertas
        /// </summary>
        public decimal TotalOpenValue { get; set; }

        /// <summary>
        /// Oportunidades ganadas este mes
        /// </summary>
        public int WonOpportunitiesThisMonth { get; set; }

        /// <summary>
        /// Valor ganado este mes
        /// </summary>
        public decimal WonValueThisMonth { get; set; }
    }

    /// <summary>
    /// Estadísticas de campañas
    /// </summary>
    public class CampaignStatistics
    {
        /// <summary>
        /// Total de campañas activas
        /// </summary>
        public int ActiveCampaigns { get; set; }

        /// <summary>
        /// Total de leads generados por campañas
        /// </summary>
        public int TotalLeadsGenerated { get; set; }

        /// <summary>
        /// Costo total de campañas activas
        /// </summary>
        public decimal TotalCampaignCost { get; set; }

        /// <summary>
        /// ROI promedio de campañas
        /// </summary>
        public decimal AverageROI { get; set; }
    }

    /// <summary>
    /// Lead reciente para el dashboard
    /// </summary>
    public class RecentLeadDto
    {
        /// <summary>
        /// ID del lead
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del lead
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Email del lead
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Estado del lead
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Oportunidad próxima a cerrar
    /// </summary>
    public class UpcomingOpportunityDto
    {
        /// <summary>
        /// ID de la oportunidad
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre de la oportunidad
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Valor de la oportunidad
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// Fecha estimada de cierre
        /// </summary>
        public DateTime EstimatedCloseDate { get; set; }

        /// <summary>
        /// Probabilidad de cierre
        /// </summary>
        public int Probability { get; set; }
    }

    /// <summary>
    /// Actividad reciente
    /// </summary>
    public class RecentActivityDto
    {
        /// <summary>
        /// ID de la actividad
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Tipo de actividad
        /// </summary>
        public string ActivityType { get; set; } = string.Empty;

        /// <summary>
        /// Descripción de la actividad
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de la actividad
        /// </summary>
        public DateTime ActivityDate { get; set; }

        /// <summary>
        /// Usuario que realizó la actividad
        /// </summary>
        public string UserName { get; set; } = string.Empty;
    }
}