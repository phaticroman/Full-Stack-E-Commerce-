
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.DTOs
{
    public class DashboardStatsDto
    {
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public IList<RecentOrderDto> RecentOrders { get; set; } = new List<RecentOrderDto>();
    }
}
