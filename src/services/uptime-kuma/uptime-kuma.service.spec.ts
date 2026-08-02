import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';
import { UptimeKumaService } from './uptime-kuma.service';

describe('UptimeKumaService', () => {
    let service: UptimeKumaService;
    let httpTesting: HttpTestingController;

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                UptimeKumaService,
                provideHttpClient(),
                provideHttpClientTesting()
            ]
        });

        service = TestBed.inject(UptimeKumaService);
        httpTesting = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
        httpTesting.verify();
    });

    it('requests and maps the Uptime Kuma response, including nullable heartbeat values', async () => {
        const statusPromise = firstValueFrom(service.getStatus());
        const request = httpTesting.expectOne(`${environment.apiBaseUrl}/api/uptime-kuma/status`);

        expect(request.request.method).toBe('GET');
        request.flush({
            RetrievedAt: '2026-08-02T18:00:00Z',
            Monitors: [{
                Id: 42,
                Name: 'Test server',
                GroupName: 'Game Servers',
                IsUp: null,
                Uptime24Hours: null,
                History: [{
                    Status: 1,
                    Time: null,
                    Ping: null,
                    Message: 'No response time available'
                }]
            }]
        });

        const status = await statusPromise;

        expect(status).toEqual({
            retrievedAt: '2026-08-02T18:00:00Z',
            monitors: [{
                id: 42,
                name: 'Test server',
                groupName: 'Game Servers',
                isUp: null,
                uptime24Hours: null,
                history: [{
                    status: 1,
                    time: null,
                    ping: null,
                    message: 'No response time available'
                }]
            }]
        });
    });

    it('normalizes missing monitor and history arrays to empty collections', async () => {
        const statusPromise = firstValueFrom(service.getStatus());
        const request = httpTesting.expectOne(`${environment.apiBaseUrl}/api/uptime-kuma/status`);

        request.flush({
            RetrievedAt: '2026-08-02T18:00:00Z',
            Monitors: [{
                Id: 7,
                Name: 'Uninitialized server',
                GroupName: 'Game Servers',
                IsUp: false,
                Uptime24Hours: 0.5
            }]
        });

        const status = await statusPromise;

        expect(status).toEqual({
            retrievedAt: '2026-08-02T18:00:00Z',
            monitors: [{
                id: 7,
                name: 'Uninitialized server',
                groupName: 'Game Servers',
                isUp: false,
                uptime24Hours: 0.5,
                history: []
            }]
        });
    });
});
