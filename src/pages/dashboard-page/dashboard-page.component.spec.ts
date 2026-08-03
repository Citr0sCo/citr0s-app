import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of, throwError } from 'rxjs';
import { DashboardPageComponent } from './dashboard-page.component';
import { SteamApiService } from '../../services/steam-api/steam-api.service';
import { UptimeKumaService } from '../../services/uptime-kuma/uptime-kuma.service';
import { ISteamUserProfile } from '../../services/steam-api/types/steam-user-profile.type';
import { ISteamUserActivity } from '../../services/steam-api/types/steam-user-activity.type';
import { ISteamOwnedGameStats } from '../../services/steam-api/types/steam-owned-game-stats.type';
import { IUptimeKumaStatus, IUptimeKumaMonitor } from '../../services/uptime-kuma/types/uptime-kuma-status.type';

describe('DashboardPageComponent', () => {
    let component: DashboardPageComponent;
    let fixture: ComponentFixture<DashboardPageComponent>;
    let mockSteamApiService: jasmine.SpyObj<SteamApiService>;
    let mockUptimeKumaService: jasmine.SpyObj<UptimeKumaService>;

    const mockProfile: ISteamUserProfile = {
        status: 1,
        name: 'Test User',
        avatarHash: 'test-hash',
        lastLogoff: 1234567890
    };

    const mockActivity: ISteamUserActivity = {
        games: [
            { playtimeLastTwoWeeksInMinutes: 60, name: 'Game 1' },
            { playtimeLastTwoWeeksInMinutes: 120, name: 'Game 2' }
        ]
    };

    const mockOwnedGameStats: ISteamOwnedGameStats = {
        totalPlayTimeHours: 50,
        totalGames: 10
    };

    const mockUptimeStatus: IUptimeKumaStatus = {
        retrievedAt: new Date().toISOString(),
        monitors: [
            {
                id: 1,
                name: 'Server 1',
                groupName: 'Game Servers',
                isUp: true,
                uptime24Hours: 0.95,
                history: [
                    { status: 1, time: new Date().toISOString(), ping: 120, message: 'OK' }
                ]
            },
            {
                id: 2,
                name: 'Server 2',
                groupName: 'Web Servers',  
                isUp: false,
                uptime24Hours: 0.85,
                history: [
                    { status: 0, time: new Date().toISOString(), ping: null, message: 'Failed' }
                ]
            }
        ]
    };

    beforeEach(async () => {
        const steamApiSpy = jasmine.createSpyObj<SteamApiService>('SteamApiService', ['getUserProfile', 'getUserProfileDecoration', 'getUserActivity', 'getOwnedGameStats']);
        const uptimeKumaSpy = jasmine.createSpyObj<UptimeKumaService>('UptimeKumaService', ['getStatus']);

        await TestBed.configureTestingModule({
            imports: [HttpClientTestingModule],
            declarations: [DashboardPageComponent],
            providers: [
                { provide: SteamApiService, useValue: steamApiSpy },
                { provide: UptimeKumaService, useValue: uptimeKumaSpy }
            ]
        }).compileComponents();

        fixture = TestBed.createComponent(DashboardPageComponent);
        component = fixture.componentInstance;
        mockSteamApiService = TestBed.inject(SteamApiService) as jasmine.SpyObj<SteamApiService>;
        mockUptimeKumaService = TestBed.inject(UptimeKumaService) as jasmine.SpyObj<UptimeKumaService>;
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    describe('ngOnInit', () => {
        it('should load steam user profile data', () => {
            mockSteamApiService.getUserProfile.and.returnValue(of(mockProfile));
            
            component.ngOnInit();
            fixture.detectChanges();

            expect(mockSteamApiService.getUserProfile).toHaveBeenCalledWith('76561198044950293');
            expect(component.profile).toBe(mockProfile);
        });

        it('should load steam user activity data', () => {
            mockSteamApiService.getUserActivity.and.returnValue(of(mockActivity));
            
            component.ngOnInit();
            fixture.detectChanges();

            expect(mockSteamApiService.getUserActivity).toHaveBeenCalledWith('76561198044950293');
            expect(component.activity).toBe(mockActivity);
        });

        it('should load steam owned game stats data', () => {
            mockSteamApiService.getOwnedGameStats.and.returnValue(of(mockOwnedGameStats));
            
            component.ngOnInit();
            fixture.detectChanges();

            expect(mockSteamApiService.getOwnedGameStats).toHaveBeenCalledWith('76561198044950293');
            expect(component.ownedGameStats).toBe(mockOwnedGameStats);
        });

        it('should handle steam API errors gracefully', () => {
            mockSteamApiService.getOwnedGameStats.and.returnValue(throwError(() => new Error('API Error')));
            
            component.ngOnInit();
            fixture.detectChanges();

            expect(component.ownedGameStats).toBeNull();
        });

        it('should load uptime kuma status data', () => {
            mockUptimeKumaService.getStatus.and.returnValue(of(mockUptimeStatus));
            
            component.ngOnInit();
            fixture.detectChanges();

            expect(mockUptimeKumaService.getStatus).toHaveBeenCalled();
            expect(component.uptimeStatus).toBe(mockUptimeStatus);
            expect(component.isUptimeKumaLoading).toBeFalse();
        });

        it('should handle uptime kuma API errors gracefully', () => {
            mockUptimeKumaService.getStatus.and.returnValue(throwError(() => new Error('API Error')));
            
            component.ngOnInit();
            fixture.detectChanges();

            expect(component.uptimeStatus).toBeNull();
            expect(component.isUptimeKumaLoading).toBeFalse();
        });
    });

    describe('Steam Status Helper Methods', () => {
        it('should return the correct status class when profile exists', () => {
            component.profile = mockProfile;
            
            const result = component.getSteamStatusClass();
            expect(result).toBe('online');
        });

        it('should return offline when profile is null', () => {
            component.profile = null;
            
            const result = component.getSteamStatusClass();
            expect(result).toBe('offline');
        });

        it('should return the correct status label when profile exists', () => {
            component.profile = mockProfile;
            
            const result = component.getSteamStatusLabel();
            expect(result).toBe('Online');
        });

        it('should return offline when profile is null', () => {
            component.profile = null;
            
            const result = component.getSteamStatusLabel();
            expect(result).toBe('Offline');
        });
    });

    describe('Uptime Status Helper Methods', () => {
        it('should correctly group monitors by name', () => {
            component.uptimeStatus = mockUptimeStatus;
            
            const groups = component.getUptimeGroups();
            expect(groups).toEqual(['Game Servers', 'Web Servers']);
        });

        it('should return empty array when there are no monitors', () => {
            component.uptimeStatus = { retrievedAt: new Date().toISOString(), monitors: [] };
            
            const groups = component.getUptimeGroups();
            expect(groups).toEqual([]);
        });

        it('should filter monitors by group name', () => {
            component.uptimeStatus = mockUptimeStatus;
            
            const gameMonitors = component.getMonitors('Game Servers');
            expect(gameMonitors.length).toBe(1);
            expect(gameMonitors[0].name).toBe('Server 1');
        });

        it('should return empty array when no monitors match group', () => {
            component.uptimeStatus = mockUptimeStatus;
            
            const result = component.getMonitors('Non Existent Group');
            expect(result).toEqual([]);
        });

        it('should calculate online monitor count correctly', () => {
            component.uptimeStatus = mockUptimeStatus;
            
            const count = component.getOnlineMonitorCount();
            expect(count).toBe(1);
        });

        it('should return zero when there are no monitors', () => {
            component.uptimeStatus = { retrievedAt: new Date().toISOString(), monitors: [] };
            
            const count = component.getOnlineMonitorCount();
            expect(count).toBe(0);
        });

        it('should return correct status class for different monitor statuses', () => {
            const onlineMonitor: IUptimeKumaMonitor = { id: 1, name: 'Test', groupName: 'Test', isUp: true, uptime24Hours: 0.95, history: [] };
            const offlineMonitor: IUptimeKumaMonitor = { id: 1, name: 'Test', groupName: 'Test', isUp: false, uptime24Hours: 0.95, history: [] };
            const unknownMonitor: IUptimeKumaMonitor = { id: 1, name: 'Test', groupName: 'Test', isUp: null, uptime24Hours: 0.95, history: [] };

            expect(component.getMonitorStatusClass(onlineMonitor)).toBe('online');
            expect(component.getMonitorStatusClass(offlineMonitor)).toBe('offline');
            expect(component.getMonitorStatusClass(unknownMonitor)).toBe('unknown');
        });

        it('should return correct status label for different monitor statuses', () => {
            const onlineMonitor: IUptimeKumaMonitor = { id: 1, name: 'Test', groupName: 'Test', isUp: true, uptime24Hours: 0.95, history: [] };
            const offlineMonitor: IUptimeKumaMonitor = { id: 1, name: 'Test', groupName: 'Test', isUp: false, uptime24Hours: 0.95, history: [] };
            const unknownMonitor: IUptimeKumaMonitor = { id: 1, name: 'Test', groupName: 'Test', isUp: null, uptime24Hours: 0.95, history: [] };

            expect(component.getMonitorStatusLabel(onlineMonitor)).toBe('Online');
            expect(component.getMonitorStatusLabel(offlineMonitor)).toBe('Offline');
            expect(component.getMonitorStatusLabel(unknownMonitor)).toBe('Unknown');
        });
    });

    describe('Display Formatting Methods', () => {
        it('should correctly calculate total playtime from minutes', () => {
            component.activity = mockActivity;
            
            const result = component.getRecentPlaytime();
            expect(result).toBe('2h 30m');
        });

        it('should return zero time when no activity data', () => {
            component.activity = null;
            
            const result = component.getRecentPlaytime();
            expect(result).toBe('0h 0m');
        });

        it('should correctly format uptime percentage', () => {
            const result = component.displayUptime(0.95);
            expect(result).toBe('95.00%');
        });

        it('should display time from minutes correctly', () => {
            const result = component.displayTimeFrom(75);
            expect(result).toBe('1h 15m');
        });

        it('should handle zero minutes correctly', () => {
            const result = component.displayTimeFrom(0);
            expect(result).toBe('0h 0m');
        });
    });

    describe('copyServerAddress', () => {
        let originalClipboard: any;

        beforeEach(() => {
            originalClipboard = navigator.clipboard;
        });

        afterEach(() => {
            // Restore original clipboard API
            (navigator as any).clipboard = originalClipboard;
        });

        it('should copy server address to clipboard successfully', async () => {
            const mockWriteText = jasmine.createSpy().and.returnValue(Promise.resolve());
            (navigator as any).clipboard = { writeText: mockWriteText };

            await component.copyServerAddress();
            
            expect(mockWriteText).toHaveBeenCalledWith('server.citr0s.com');
            expect(component.serverAddressCopied).toBeTrue();
        });
    });
});
