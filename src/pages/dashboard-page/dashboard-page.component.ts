import {AfterViewInit, Component, OnDestroy, OnInit} from '@angular/core';
import {Subject} from 'rxjs';

@Component({
    selector: 'welcome-page',
    templateUrl: './dashboard-page.component.html',
    styleUrls: ['./dashboard-page.component.scss'],
    standalone: false
})
export class DashboardPageComponent implements AfterViewInit, OnInit, OnDestroy {

    public currentYear: number = 0;

    private readonly _destroy: Subject<void> = new Subject();

    public ngOnInit(): void {
        this.currentYear = new Date().getFullYear();
    }

    public ngAfterViewInit(): void {

    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
