import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
    IUptimeKumaHeartbeat,
    IUptimeKumaMonitor,
    IUptimeKumaStatus
} from './types/uptime-kuma-status.type';

@Injectable()
export class UptimeKumaService {
    private readonly _httpClient: HttpClient;

    constructor(httpClient: HttpClient) {
        this._httpClient = httpClient;
    }

    public getStatus(): Observable<IUptimeKumaStatus> {
        return this._httpClient.get<any>(`${environment.apiBaseUrl}/api/uptime-kuma/status`)
            .pipe(
                map((response: any): IUptimeKumaStatus => ({
                    retrievedAt: response.RetrievedAt,
                    monitors: (response.Monitors ?? []).map((monitor: any): IUptimeKumaMonitor => ({
                        id: monitor.Id,
                        name: monitor.Name,
                        groupName: monitor.GroupName,
                        isUp: monitor.IsUp,
                        uptime24Hours: monitor.Uptime24Hours,
                        history: (monitor.History ?? []).map((heartbeat: any): IUptimeKumaHeartbeat => ({
                            status: heartbeat.Status,
                            time: heartbeat.Time,
                            ping: heartbeat.Ping,
                            message: heartbeat.Message
                        }))
                    }))
                }))
            );
    }
}
