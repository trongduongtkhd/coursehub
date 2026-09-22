import { Component, OnInit } from '@angular/core';
import {
  ApexAxisChartSeries,
  ApexChart,
  ApexXAxis,
  ApexDataLabels,
  ApexStroke,
  ApexPlotOptions,
} from 'ng-apexcharts';
import { DashboardService } from 'src/app/core/services/dashboard.service';
import { DashboardStats } from 'src/app/core/models/dashboard.model';

export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  dataLabels?: ApexDataLabels;
  stroke?: ApexStroke;
  plotOptions?: ApexPlotOptions;
  colors?: string[];
};

@Component({
  selector: 'app-dashboard-home',
  templateUrl: './dashboard-home.component.html',
  styleUrls: ['./dashboard-home.component.scss'],
})
export class DashboardHomeComponent implements OnInit {
  stats: DashboardStats | null = null;
  loading = true;

  enrollmentChart: Partial<ChartOptions> = {};
  topCoursesChart: Partial<ChartOptions> = {};

  private monthNames = [
    'T1',
    'T2',
    'T3',
    'T4',
    'T5',
    'T6',
    'T7',
    'T8',
    'T9',
    'T10',
    'T11',
    'T12',
  ];

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.dashboardService.getStats().subscribe({
      next: (data) => {
        this.stats = data;
        this.buildCharts(data);
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  private buildCharts(data: DashboardStats): void {
    this.enrollmentChart = {
      series: [
        {
          name: 'Lượt đăng ký',
          data: data.enrollmentsByMonth.map((m) => m.count),
        },
      ],
      chart: { type: 'line', height: 280, toolbar: { show: false } },
      xaxis: {
        categories: data.enrollmentsByMonth.map(
          (m) => `${this.monthNames[m.month - 1]}/${m.year}`,
        ),
      },
      stroke: { curve: 'smooth', width: 3 },
      dataLabels: { enabled: false },
      colors: ['#0F5C5C'],
    };

    this.topCoursesChart = {
      series: [
        {
          name: 'Học viên',
          data: data.topCourses.map((c) => c.enrollmentCount),
        },
      ],
      chart: { type: 'bar', height: 280, toolbar: { show: false } },
      xaxis: { categories: data.topCourses.map((c) => c.title) },
      plotOptions: { bar: { horizontal: true, borderRadius: 4 } },
      dataLabels: { enabled: true },
      colors: ['#E8A33D'],
    };
  }
}
