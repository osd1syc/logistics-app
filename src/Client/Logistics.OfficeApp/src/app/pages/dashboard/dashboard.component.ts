import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {CompanyStatsComponent, TruckStatsTableComponent} from './components';
import {GrossesBarchartComponent} from '@shared/components';


@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: [],
  standalone: true,
  imports: [
    CommonModule,
    TruckStatsTableComponent,
    GrossesBarchartComponent,
    CompanyStatsComponent,
  ],
})
export class DashboardComponent {
  constructor() {}
}
