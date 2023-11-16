import {Component, OnInit} from '@angular/core';
import {NgIf, CurrencyPipe} from '@angular/common';
import {RouterLink} from '@angular/router';
import {ChartModule} from 'primeng/chart';
import {SkeletonModule} from 'primeng/skeleton';
import {ButtonModule} from 'primeng/button';
import {TooltipModule} from 'primeng/tooltip';
import {TableModule} from 'primeng/table';
import {SharedModule} from 'primeng/api';
import {CardModule} from 'primeng/card';
import {AppConfig} from '@configs';
import {DailyGrosses, Load} from '@core/models';
import {ApiService} from '@core/services';
import {TrucksMapComponent} from '@shared/components';
import {AddressPipe, DistanceUnitPipe} from '@shared/pipes';
import {DateUtils, DistanceConverter} from '@shared/utils';
import {NotificationsPanelComponent} from './components';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  standalone: true,
  imports: [
    CardModule,
    SharedModule,
    TableModule,
    RouterLink,
    TooltipModule,
    ButtonModule,
    NgIf,
    SkeletonModule,
    ChartModule,
    CurrencyPipe,
    DistanceUnitPipe,
    TrucksMapComponent,
    NotificationsPanelComponent,
    AddressPipe,
  ],
})
export class HomeComponent implements OnInit {
  public readonly accessToken = AppConfig.mapboxToken;
  public todayGross = 0;
  public weeklyGross = 0;
  public weeklyDistance = 0;
  public weeklyRpm = 0;
  public isLoadingLoadsData = false;
  public isLoadingChartData = false;
  public loads: Load[] = [];
  public chartData: unknown;
  public chartOptions: unknown;

  constructor(private readonly apiService: ApiService) {
    this.chartData = {
      labels: [],
      datasets: [
        {
          label: 'Daily Gross',
          data: [],
        },
      ],
    },

    this.chartOptions = {
      plugins: {
        legend: {
          display: false,
        },
      },
    };
  }

  ngOnInit() {
    this.fetchActiveLoads();
    this.fetchLastTenDaysGross();
  }

  private fetchActiveLoads() {
    this.isLoadingLoadsData = true;

    this.apiService.getLoads({orderBy: '-dispatchedDate'}, true).subscribe((result) => {
      if (result.isSuccess && result.data) {
        this.loads = result.data;
      }

      this.isLoadingLoadsData = false;
    });
  }

  private fetchLastTenDaysGross() {
    this.isLoadingChartData = true;
    const oneWeekAgo = DateUtils.daysAgo(7);

    this.apiService.getDailyGrosses(oneWeekAgo).subscribe((result) => {
      if (result.isSuccess && result.data) {
        const grosses = result.data;

        this.weeklyGross = grosses.totalGross;
        this.weeklyDistance = grosses.totalDistance;
        this.weeklyRpm = this.weeklyGross / DistanceConverter.metersTo(this.weeklyDistance, 'mi');
        this.drawChart(grosses);
        this.calcTodayGross(grosses);
      }

      this.isLoadingChartData = false;
    });
  }

  private drawChart(grosses: DailyGrosses) {
    const labels: Array<string> = [];
    const data: Array<number> = [];

    grosses.data.forEach((i) => {
      labels.push(DateUtils.toLocaleDate(i.date));
      data.push(i.gross);
    });

    this.chartData = {
      labels: labels,
      datasets: [
        {
          label: 'Daily Gross',
          data: data,
          fill: true,
          tension: 0.4,
          borderColor: '#405a83',
          backgroundColor: '#88a5d3',
        },
      ],
    };
  }

  private calcTodayGross(grosses: DailyGrosses) {
    const today = new Date();
    let totalGross = 0;

    grosses.data
      .filter((i) => DateUtils.dayOfMonth(i.date) === today.getDate())
      .forEach((i) => totalGross += i.gross);

    this.todayGross = totalGross;
  }
}
