import {Component, OnDestroy, OnInit, ChangeDetectionStrategy} from '@angular/core';
import {Subject, takeUntil} from 'rxjs';
import {SteamApiService} from "../../services/steam-api/steam-api.service";
import {ISteamUserProfile} from "../../services/steam-api/types/steam-user-profile.type";
import {ISteamUserProfileDecoration} from "../../services/steam-api/types/steam-user-profile-decoration.type";
import {ISteamUserActivity} from "../../services/steam-api/types/steam-user-activity.type";
import {ISteamOwnedGameStats} from "../../services/steam-api/types/steam-owned-game-stats.type";
import {SteamUserStatus} from "../../services/steam-api/types/steam-user-status.type";
import {UptimeKumaService} from "../../services/uptime-kuma/uptime-kuma.service";
import {IUptimeKumaMonitor, IUptimeKumaStatus} from "../../services/uptime-kuma/types/uptime-kuma-status.type";

@Component({
    selector: 'welcome-page',
    templateUrl: './dashboard-page.component.html',
    styleUrls: ['./dashboard-page.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class DashboardPageComponent implements OnInit, OnDestroy {

    public currentYear: number = 0;
    public steamUserId: string = '76561198044950293';
    public profile: ISteamUserProfile | null = null;
    public profileDecoration: ISteamUserProfileDecoration | null = null;
    public activity: ISteamUserActivity | null = null;
    public ownedGameStats: ISteamOwnedGameStats | null = null;
    public uptimeStatus: IUptimeKumaStatus | null = null;
    public isUptimeKumaLoading: boolean = true;
    public serverAddressCopied: boolean = false;
    public SteamUserStatus = SteamUserStatus;

    private readonly _gameServersAddress: string = 'server.citr0s.com';

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _steamApiService: SteamApiService;
    private readonly _uptimeKumaService: UptimeKumaService;

    constructor(steamApiService: SteamApiService, uptimeKumaService: UptimeKumaService) {
        this._steamApiService = steamApiService;
        this._uptimeKumaService = uptimeKumaService;
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

        this._steamApiService
            .getOwnedGameStats(this.steamUserId)
            .pipe(takeUntil(this._destroy))
            .subscribe({
                next: (stats) => {
                    this.ownedGameStats = stats;
                },
                error: () => {
                    this.ownedGameStats = null;
                }
            });

        this._uptimeKumaService
            .getStatus()
            .pipe(takeUntil(this._destroy))
            .subscribe({
                next: (status) => {
                    this.uptimeStatus = status;
                    this.isUptimeKumaLoading = false;
                },
                error: () => {
                    this.uptimeStatus = null;
                    this.isUptimeKumaLoading = false;
                }
            });
    }


    public getSteamStatusClass(): string {
        return this.getSteamStatusLabel().toLowerCase();
    }

    public getSteamStatusLabel(): string {
        if (!this.profile)
            return 'Offline';

        return SteamUserStatus[this.profile.status];
    }

    public getRecentPlaytime(): string {
        const totalMinutes = this.activity?.games.reduce(
            (total, game) => total + game.playtimeLastTwoWeeksInMinutes,
            0
        ) ?? 0;

        return this.displayTimeFrom(totalMinutes);
    }

    public getUptimeGroups(): string[] {
        return [...new Set(this.uptimeStatus?.monitors.map((monitor) => monitor.groupName) ?? [])];
    }

    public getMonitors(groupName: string): IUptimeKumaMonitor[] {
        return this.uptimeStatus?.monitors.filter((monitor) => monitor.groupName === groupName) ?? [];
    }

    public getOnlineMonitorCount(): number {
        return this.uptimeStatus?.monitors.filter((monitor) => monitor.isUp === true).length ?? 0;
    }

    public getMonitorStatusClass(monitor: IUptimeKumaMonitor): string {
        if (monitor.isUp === true)
            return 'online';

        if (monitor.isUp === false)
            return 'offline';

        return 'unknown';
    }

    public getMonitorStatusLabel(monitor: IUptimeKumaMonitor): string {
        if (monitor.isUp === true)
            return 'Online';

        if (monitor.isUp === false)
            return 'Offline';

        return 'Unknown';
    }

    public getHeartbeatStatusClass(status: number): string {
        switch (status) {
            case 1:
                return 'up';
            case 0:
                return 'down';
            case 2:
                return 'pending';
            case 3:
                return 'maintenance';
            default:
                return 'unknown';
        }
    }

    public displayUptime(uptime: number): string {
        return `${(uptime * 100).toFixed(2)}%`;
    }

    public displayTimeFrom(totalMinutes: number): string {
        const hours = Math.floor(totalMinutes / 60);
        const minutes = totalMinutes % 60;

        return `${hours}h ${minutes}m`;
    }

    public async copyServerAddress(): Promise<void> {
        try {
            await navigator.clipboard.writeText(this._gameServersAddress);
            this.serverAddressCopied = true;
            window.setTimeout(() => this.serverAddressCopied = false, 1800);
        } catch (_) {
            this.serverAddressCopied = false;
        }
    }

    public ngOnDestroy(): void {
        this._destroy.next();
        this._destroy.complete();
    }
}
