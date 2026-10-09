import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-terminal-button',
  templateUrl: './terminal-button.html',
  styleUrl: './terminal-button.css',
})
export class TerminalButton {
  readonly type = input<'button' | 'submit'>('button');
  readonly disabled = input(false);
  readonly pressed = output<void>();
}
