import { Component, inject } from "@angular/core";
import { SidebarChildItem } from "../sidebar-item/sidebar-item.component";
import { AuthService } from "../../auth/services/AuthService ";
import { RouterLink } from "@angular/router";

@Component({
    imports: [SidebarChildItem, RouterLink],
    templateUrl: './sidebar.component.html',
    selector: 'sidebar-item'
})
export class SidebarItem{
    authservice = inject(AuthService)

    OnLogout(event: Event){
        event.preventDefault(); // Prevenir el comportamiento por defecto
        event.stopPropagation(); // Evitar propagación del evento
        this.authservice.logout()
    }
}