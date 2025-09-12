import { ChangeDetectionStrategy, Component } from '@angular/core';
import { NavbarItem } from "../../app/admin-layout/navbar/navbar.component";
import { SidebarItem } from "../../app/admin-layout/sidebar/sidebar.component";
import { FooterItem } from "../../app/admin-layout/footer/footer.component";
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-main-layout-component',
  imports: [NavbarItem, SidebarItem, FooterItem, RouterOutlet],
  templateUrl: './main-layout.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MainLayoutComponent { }
