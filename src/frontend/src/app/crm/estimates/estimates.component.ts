import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-estimates',
  imports: [],
  template: `<p>estimates works!</p>`,
  styleUrl: './estimates.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EstimatesComponent { }
