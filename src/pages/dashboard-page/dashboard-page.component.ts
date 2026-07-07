import {AfterViewInit, Component, OnDestroy, OnInit} from '@angular/core';
import {map, Subject, takeUntil} from 'rxjs';
import {SteamApiService} from "../../services/steam-api/steam-api.service";
import {ISteamUserProfile} from "../../services/steam-api/types/steam-user-profile.type";
import {ISteamUserProfileDecoration} from "../../services/steam-api/types/steam-user-profile-decoration.type";
import {ISteamUserActivity} from "../../services/steam-api/types/steam-user-activity.type";
import {SteamUserStatus} from "../../services/steam-api/types/steam-user-status.type";

@Component({
    selector: 'welcome-page',
    templateUrl: './dashboard-page.component.html',
    styleUrls: ['./dashboard-page.component.scss'],
    standalone: false
})
export class DashboardPageComponent implements AfterViewInit, OnInit, OnDestroy {

    public currentYear: number = 0;
    public steamUserId: string = '76561198044950293';
    public profile: ISteamUserProfile | null = null;
    public profileDecoration: ISteamUserProfileDecoration | null = null;
    public activity: ISteamUserActivity | null = null;
    public SteamUserStatus = SteamUserStatus;

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _steamApiService: SteamApiService;

    constructor(steamApiService: SteamApiService) {
        this._steamApiService = steamApiService;
    }

    public ngOnInit(): void {
        this.currentYear = new Date().getFullYear();

        this._steamApiService
            .getUserProfile(this.steamUserId)
            .pipe(takeUntil(this._destroy))
            .subscribe((profile) => {
                this.profile = profile;
            });

        this._steamApiService
            .getUserProfileDecoration(this.steamUserId)
            .pipe(takeUntil(this._destroy))
            .subscribe((decoration) => {
                this.profileDecoration = decoration;
            });

        this._steamApiService
            .getUserActivity(this.steamUserId)
            .pipe(takeUntil(this._destroy))
            .subscribe((activity) => {
                this.activity = activity;
            });
    }

    public ngAfterViewInit(): void {

    }

    public displayTimeFrom(totalMinutes: number): string {
        const hours = Math.floor(totalMinutes / 60);
        const minutes = totalMinutes % 60;

        return `${hours}h ${minutes}m`;
}

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
