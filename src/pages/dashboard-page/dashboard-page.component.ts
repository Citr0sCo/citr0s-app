import {AfterViewInit, Component, OnDestroy, OnInit} from '@angular/core';
import {Subject, takeUntil} from "rxjs";

@Component({
    selector: 'welcome-page',
    templateUrl: './dashboard-page.component.html',
    styleUrls: ['./dashboard-page.component.scss'],
    standalone: false
})
export class DashboardPageComponent implements AfterViewInit, OnInit, OnDestroy {

    private readonly _destroy: Subject<void> = new Subject();


    public ngOnInit(): void {
    
        window.addEventListener('DOMContentLoaded', function(e) {
            var iFrame = document.querySelector( '.status-iframe' );
            this.resizeIFrameToFitContent( iFrame );
        } );
    }

    public ngAfterViewInit(): void {

    }

    public resizeIFrameToFitContent(iFrame): void {
        iFrame.width  = iFrame.contentWindow.document.body.scrollWidth;
        iFrame.height = iFrame.contentWindow.document.body.scrollHeight;
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
