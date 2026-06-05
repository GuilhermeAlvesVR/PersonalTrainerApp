const API_BASE = localStorage.getItem('apiBase') || 'https://personaltrainerapp-myci.onrender.com';
const dias = ['Segunda-feira', 'Terça-feira', 'Quarta-feira', 'Quinta-feira', 'Sexta-feira'];
let horarios = ['06:00', '07:00', '08:00', '09:00', '10:00', '15:00', '16:00', '17:00', '18:00', '19:00', '20:00'];
let alunos = [];
let treinos = [];
let avaliacoes = [];
let agenda = [];
let alunoSelecionado = null;
let filtro = 'todos';
let destaqueAluno = '';

const $ = (id) => document.getElementById(id);

async function api(path, options = {}) {
  const response = await fetch(`${API_BASE}${path}`, {
    headers: { 'Content-Type': 'application/json', ...(options.headers || {}) },
    ...options
  });
  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || `Erro ${response.status}`);
  }
  if (response.status === 204) return null;
  return response.json();
}

function showStatus(message, type = '') {
  const status = $('status');
  status.textContent = message;
  status.className = `status ${type}`;
  clearTimeout(showStatus.timer);
  showStatus.timer = setTimeout(() => status.classList.add('hidden'), 4200);
}

function num(value) { return Number(String(value || '').replace(',', '.')) || 0; }
function fmt(value, suffix = '') { return value ? `${value}${suffix}` : '-'; }
function dateFmt(value) { return value ? new Date(value).toLocaleDateString('pt-BR') : '-'; }

async function carregarAlunos() {
  alunos = await api('/api/Alunos');
  renderAlunos();
  if (alunoSelecionado) {
    alunoSelecionado = alunos.find(a => a.id === alunoSelecionado.id) || null;
    if (alunoSelecionado) await selecionarAluno(alunoSelecionado.id);
  }
}

function renderAlunos() {
  const busca = $('buscaAluno').value.trim().toLowerCase();
  const lista = alunos.filter(a => {
    const passaBusca = !busca || a.nome.toLowerCase().includes(busca);
    const passaFiltro = filtro === 'todos' || (filtro === 'ativos' ? a.ativo : !a.ativo);
    return passaBusca && passaFiltro;
  });
  $('listaAlunos').innerHTML = lista.map(a => `
    <div class="student-item ${alunoSelecionado?.id === a.id ? 'active' : ''}" data-id="${a.id}">
      <strong>${a.nome}</strong>
      <span>${a.idade || 0} anos · ${a.ativo ? 'Ativo' : 'Inativo'}</span>
    </div>
  `).join('') || '<p class="empty-state">Nenhum aluno encontrado.</p>';
  document.querySelectorAll('.student-item').forEach(el => el.onclick = () => selecionarAluno(Number(el.dataset.id)));
}

async function selecionarAluno(id) {
  alunoSelecionado = alunos.find(a => a.id === id);
  if (!alunoSelecionado) return;
  $('emptyState').classList.add('hidden');
  $('alunoDetalhe').classList.remove('hidden');
  $('detalheNome').textContent = alunoSelecionado.nome;
  $('detalheResumo').textContent = alunoSelecionado.objetivo || 'Sem objetivo informado';
  $('detalhePeso').textContent = fmt(alunoSelecionado.peso, ' kg');
  $('detalheAltura').textContent = fmt(alunoSelecionado.altura, ' m');
  $('detalheUltimoTreino').textContent = alunoSelecionado.ultimoTreinoRealizado || '-';
  $('detalheAtivo').textContent = alunoSelecionado.ativo ? 'Ativo' : 'Inativo';
  renderAlunos();
  await Promise.all([carregarTreinos(), carregarAvaliacoes()]);
}

async function carregarTreinos() {
  treinos = await api(`/api/Treinos/aluno/${alunoSelecionado.id}`);
  renderTreinos();
}

function renderTreinos() {
  $('listaTreinos').innerHTML = treinos.map(t => `
    <article class="item-card">
      <div class="panel-header compact">
        <h4>${t.letraTreino} - ${t.nomeTreino}</h4>
        <div class="actions">
          <button data-action="feito" data-id="${t.id}">Feito</button>
          <button data-action="editar" data-id="${t.id}">Editar</button>
          <button data-action="excluir" data-id="${t.id}" class="danger">Excluir</button>
        </div>
      </div>
      <ol class="exercise-list">${(t.exercicios || []).map(e => `<li>${e.nome} · ${e.series}x${e.repeticoes} · ${e.carga || '-'}</li>`).join('')}</ol>
    </article>
  `).join('') || '<p class="empty-state">Nenhum treino cadastrado.</p>';
  document.querySelectorAll('[data-action="editar"]').forEach(b => b.onclick = () => abrirTreino(Number(b.dataset.id)));
  document.querySelectorAll('[data-action="excluir"]').forEach(b => b.onclick = () => excluirTreino(Number(b.dataset.id)));
  document.querySelectorAll('[data-action="feito"]').forEach(b => b.onclick = () => marcarTreinoFeito(Number(b.dataset.id)));
}

async function carregarAvaliacoes() {
  avaliacoes = await api(`/api/Avaliacoes/aluno/${alunoSelecionado.id}`);
  renderAvaliacoes();
}

function renderAvaliacoes() {
  $('listaAvaliacoes').innerHTML = avaliacoes.map(a => `
    <article class="item-card">
      <h4>${dateFmt(a.data)}</h4>
      <p>Peso: ${fmt(a.peso, ' kg')} · Peito: ${fmt(a.peito)} · Cintura: ${fmt(a.cintura)}</p>
      <p>Abdomen: ${fmt(a.abdomen)} · Quadril: ${fmt(a.quadril)} · Braco: ${fmt(a.braco)} · Coxa: ${fmt(a.coxa)}</p>
      <p>${a.observacoes || ''}</p>
    </article>
  `).join('') || '<p class="empty-state">Nenhuma avaliacao cadastrada.</p>';
}

function abrirAluno(aluno = null) {
  $('alunoDialogTitulo').textContent = aluno ? 'Editar aluno' : 'Novo aluno';
  $('alunoId').value = aluno?.id || '';
  $('alunoNome').value = aluno?.nome || '';
  $('alunoIdade').value = aluno?.idade || '';
  $('alunoPeso').value = aluno?.peso || '';
  $('alunoAltura').value = aluno?.altura || '';
  $('alunoObjetivo').value = aluno?.objetivo || '';
  $('alunoObservacoes').value = aluno?.observacoes || '';
  $('alunoAtivo').checked = aluno?.ativo ?? true;
  $('alunoDialog').showModal();
}

async function salvarAluno(event) {
  event.preventDefault();
  const id = $('alunoId').value;
  const payload = {
    id: id ? Number(id) : 0,
    nome: $('alunoNome').value.trim(),
    idade: num($('alunoIdade').value),
    peso: num($('alunoPeso').value),
    altura: num($('alunoAltura').value),
    objetivo: $('alunoObjetivo').value.trim(),
    observacoes: $('alunoObservacoes').value.trim(),
    ativo: $('alunoAtivo').checked,
    ultimoTreinoRealizado: alunoSelecionado?.ultimoTreinoRealizado || '',
    personalEmpresaId: 1
  };
  if (id) await api(`/api/Alunos/${id}`, { method: 'PUT', body: JSON.stringify(payload) });
  else await api('/api/Alunos', { method: 'POST', body: JSON.stringify(payload) });
  $('alunoDialog').close();
  showStatus('Aluno salvo.', 'success');
  await carregarAlunos();
}

async function excluirAluno() {
  if (!alunoSelecionado || !confirm(`Excluir ${alunoSelecionado.nome}?`)) return;
  await api(`/api/Alunos/${alunoSelecionado.id}`, { method: 'DELETE' });
  alunoSelecionado = null;
  $('alunoDetalhe').classList.add('hidden');
  $('emptyState').classList.remove('hidden');
  showStatus('Aluno excluido.', 'success');
  await carregarAlunos();
}

function abrirTreino(id = 0) {
  const treino = treinos.find(t => t.id === id);
  $('treinoDialogTitulo').textContent = treino ? 'Editar treino' : 'Novo treino';
  $('treinoId').value = treino?.id || '';
  $('treinoLetra').value = treino?.letraTreino || '';
  $('treinoNome').value = treino?.nomeTreino || '';
  $('exerciciosEditor').innerHTML = '';
  (treino?.exercicios?.length ? treino.exercicios : [{ nome: '', series: '', repeticoes: '', carga: '' }]).forEach(addExercicioRow);
  $('treinoDialog').showModal();
}

function addExercicioRow(exercicio = {}) {
  const row = document.createElement('div');
  row.className = 'exercise-row';
  row.innerHTML = `
    <input placeholder="Exercicio" value="${exercicio.nome || ''}">
    <input type="number" placeholder="Series" value="${exercicio.series || ''}">
    <input type="number" placeholder="Repeticoes" value="${exercicio.repeticoes || ''}">
    <input placeholder="Carga" value="${exercicio.carga || ''}">
    <button type="button" class="danger">Remover</button>
  `;
  row.querySelector('button').onclick = () => row.remove();
  $('exerciciosEditor').appendChild(row);
}

async function salvarTreino(event) {
  event.preventDefault();
  const exercicios = [...document.querySelectorAll('.exercise-row')].map(row => {
    const inputs = row.querySelectorAll('input');
    return { nome: inputs[0].value.trim(), series: num(inputs[1].value), repeticoes: num(inputs[2].value), carga: inputs[3].value.trim() };
  }).filter(e => e.nome);
  const payload = {
    id: Number($('treinoId').value || 0),
    letraTreino: $('treinoLetra').value.trim(),
    nomeTreino: $('treinoNome').value.trim(),
    alunoId: alunoSelecionado.id,
    exercicios
  };
  await api('/api/Treinos', { method: 'POST', body: JSON.stringify(payload) });
  $('treinoDialog').close();
  showStatus('Treino salvo.', 'success');
  await carregarTreinos();
}

async function excluirTreino(id) {
  if (!confirm('Excluir este treino?')) return;
  await api(`/api/Treinos/${id}`, { method: 'DELETE' });
  showStatus('Treino excluido.', 'success');
  await carregarTreinos();
}

async function marcarTreinoFeito(id) {
  const treino = treinos.find(t => t.id === id);
  const texto = `${treino.letraTreino} - ${treino.nomeTreino}`;
  await api(`/api/Alunos/${alunoSelecionado.id}/ultimo-treino`, { method: 'PUT', body: JSON.stringify(texto) });
  alunoSelecionado.ultimoTreinoRealizado = texto;
  showStatus('Ultimo treino atualizado.', 'success');
  await carregarAlunos();
}

async function salvarAvaliacao(event) {
  event.preventDefault();
  const payload = {
    alunoId: alunoSelecionado.id,
    peso: num($('avPeso').value), peito: num($('avPeito').value), cintura: num($('avCintura').value),
    abdomen: num($('avAbdomen').value), quadril: num($('avQuadril').value), braco: num($('avBraco').value),
    coxa: num($('avCoxa').value), observacoes: $('avObservacoes').value.trim()
  };
  await api('/api/Avaliacoes', { method: 'POST', body: JSON.stringify(payload) });
  $('avaliacaoDialog').close();
  $('avaliacaoForm').reset();
  showStatus('Avaliacao salva.', 'success');
  await carregarAvaliacoes();
}

async function carregarAgenda() {
  agenda = await api('/api/AgendaHorarios');
  if (!agenda.length) agenda = criarAgendaVazia();
  const salvos = [...new Set(agenda.map(a => a.horario).filter(Boolean))];
  if (salvos.length) horarios = salvos;
  renderAgenda();
}

function criarAgendaVazia() {
  return dias.flatMap(diaSemana => horarios.map(horario => ({ id: 0, diaSemana, horario, conteudo: '' })));
}

function getAgendaItem(diaSemana, horario) {
  let item = agenda.find(a => a.diaSemana === diaSemana && a.horario === horario);
  if (!item) {
    item = { id: 0, diaSemana, horario, conteudo: '' };
    agenda.push(item);
  }
  return item;
}

function splitLinhas(texto) { return String(texto || '').replace(/\r\n/g, '\n').split('\n'); }
function setLinha(item, idx, valor) {
  const linhas = splitLinhas(item.conteudo);
  while (linhas.length <= idx) linhas.push('');
  linhas[idx] = valor.trim();
  while (linhas.length && !linhas[linhas.length - 1].trim()) linhas.pop();
  item.conteudo = linhas.join('\n');
}

function renderAgenda() {
  $('agendaTabela').innerHTML = dias.map(dia => {
    const itens = horarios.map(h => getAgendaItem(dia, h));
    const linhas = Math.max(5, Math.max(...itens.map(i => splitLinhas(i.conteudo).length)) + 1);
    return `<table class="day-table"><tbody>
      <tr><th class="day-name" rowspan="${linhas + 1}">${dia}</th>${horarios.map((h, i) => `<th class="time-cell"><input data-time-index="${i}" value="${h}"></th>`).join('')}</tr>
      ${Array.from({ length: linhas }, (_, linha) => `<tr>${itens.map(item => {
        const valor = splitLinhas(item.conteudo)[linha] || '';
        const highlight = destaqueAluno && valor.trim().toLowerCase() === destaqueAluno.toLowerCase();
        return `<td class="student-cell ${highlight ? 'highlight' : ''}"><input data-dia="${dia}" data-horario="${item.horario}" data-linha="${linha}" value="${valor}"></td>`;
      }).join('')}</tr>`).join('')}
    </tbody></table>`;
  }).join('');

  document.querySelectorAll('[data-time-index]').forEach(input => {
    input.onchange = () => alterarHorario(Number(input.dataset.timeIndex), input.value.trim());
  });
  document.querySelectorAll('.student-cell input').forEach(input => {
    input.oninput = () => setLinha(getAgendaItem(input.dataset.dia, input.dataset.horario), Number(input.dataset.linha), input.value);
    input.onclick = () => { if (input.value.trim()) { destaqueAluno = input.value.trim(); renderAgenda(); } };
  });
}

function alterarHorario(index, novo) {
  if (!novo) return renderAgenda();
  const antigo = horarios[index];
  horarios[index] = novo;
  agenda.filter(a => a.horario === antigo).forEach(a => a.horario = novo);
  renderAgenda();
}

async function salvarAgenda() {
  await api('/api/AgendaHorarios', { method: 'PUT', body: JSON.stringify(agenda) });
  showStatus('Agenda salva.', 'success');
}

function setup() {
  document.querySelectorAll('.tab').forEach(tab => tab.onclick = async () => {
    document.querySelectorAll('.tab').forEach(t => t.classList.remove('active'));
    document.querySelectorAll('.view').forEach(v => v.classList.remove('active'));
    tab.classList.add('active');
    $(`${tab.dataset.view}View`).classList.add('active');
    if (tab.dataset.view === 'agenda' && !agenda.length) await carregarAgenda();
  });
  $('buscaAluno').oninput = renderAlunos;
  document.querySelectorAll('.filter').forEach(btn => btn.onclick = () => {
    document.querySelectorAll('.filter').forEach(b => b.classList.remove('active'));
    btn.classList.add('active'); filtro = btn.dataset.filter; renderAlunos();
  });
  $('novoAlunoBtn').onclick = () => abrirAluno();
  $('editarAlunoBtn').onclick = () => abrirAluno(alunoSelecionado);
  $('excluirAlunoBtn').onclick = excluirAluno;
  $('novoTreinoBtn').onclick = () => abrirTreino();
  $('adicionarExercicioBtn').onclick = () => addExercicioRow();
  $('novaAvaliacaoBtn').onclick = () => $('avaliacaoDialog').showModal();
  $('salvarAgendaBtn').onclick = salvarAgenda;
  $('limparDestaqueAgendaBtn').onclick = () => { destaqueAluno = ''; renderAgenda(); };
  $('alunoForm').onsubmit = salvarAluno;
  $('treinoForm').onsubmit = salvarTreino;
  $('avaliacaoForm').onsubmit = salvarAvaliacao;
  document.querySelectorAll('[data-close]').forEach(btn => btn.onclick = () => btn.closest('dialog').close());
}

setup();
carregarAlunos().catch(err => showStatus(`Nao foi possivel carregar os dados: ${err.message}`, 'error'));
