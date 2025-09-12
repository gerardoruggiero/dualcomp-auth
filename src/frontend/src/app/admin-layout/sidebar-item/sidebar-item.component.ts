import { Component, input, signal } from "@angular/core";

@Component({
    imports:[],
    templateUrl: './sidebar-item.component.html',
    selector: 'sidebar-child-item'
})
export class SidebarChildItem{
    TextValue = input.required<String>();
    Icon = input("");
}