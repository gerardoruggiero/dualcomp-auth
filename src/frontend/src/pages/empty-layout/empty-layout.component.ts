import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// empty-layout.component.ts
@Component({
  selector: 'app-empty-layout',
  imports:[RouterOutlet],
  templateUrl: './empty-layout.component.html'
})
export class EmptyLayoutComponent {}
