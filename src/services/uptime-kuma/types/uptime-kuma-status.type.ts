export interface IUptimeKumaStatus {
    retrievedAt: string;
    monitors: IUptimeKumaMonitor[];
}

export interface IUptimeKumaMonitor {
    id: number;
    name: string;
    groupName: string;
    isUp: boolean | null;
    uptime24Hours: number | null;
    history: IUptimeKumaHeartbeat[];
}

export interface IUptimeKumaHeartbeat {
    status: number;
    time: string | null;
    ping: number | null;
    message: string;
}
