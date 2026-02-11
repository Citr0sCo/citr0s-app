import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {DashboardPageComponent} from "../pages/dashboard-page/dashboard-page.component";

const routes: Routes = [
    {
        path: 'welcome',
        component: DashboardPageComponent
    },
    { path: '**', pathMatch: 'full', redirectTo: 'welcome' }
];

@NgModule({
    imports: [
        RouterModule.forRoot(routes, { useHash: true })
    ],
    exports: [
        RouterModule
    ]
})
export class AppRoutingModule {
}
