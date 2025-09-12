import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-lead',
  imports: [],
  template: `<p>lead works!</p>`,
  styleUrl: './lead.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LeadComponent { }
