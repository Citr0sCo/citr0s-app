import { NgModule, isDevMode } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ServiceWorkerModule } from '@angular/service-worker';
import { TimeagoModule } from 'ngx-timeago';
import { DashboardPageComponent } from "../pages/dashboard-page/dashboard-page.component";

@NgModule({
    declarations: [
        AppComponent,
        DashboardPageComponent,
    ],
    bootstrap: [AppComponent],
    imports: [
        BrowserModule,
        AppRoutingModule,
        ReactiveFormsModule,
        FormsModule,
        ServiceWorkerModule.register('ngsw-worker.js', {
            enabled: !isDevMode(),
            registrationStrategy: 'registerWhenStable:30000'
        }),
        TimeagoModule.forRoot(),
    ], providers: [
        provideHttpClient(withInterceptorsFromDi())
    ]
})
export class AppModule {
}
